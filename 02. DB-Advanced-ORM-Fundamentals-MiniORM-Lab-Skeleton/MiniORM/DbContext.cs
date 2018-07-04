using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace MiniORM
{
    // DbContext – Database context class, responsible for retrieving entities from the database 
    // and mapping the relations between them (through so-called navigation properties).

    public abstract class DbContext
    {
        private readonly DatabaseConnection connection;

        private readonly Dictionary<Type, PropertyInfo> dbSetProperties;

        internal static readonly Type[] AllowedSqlTypes =
        {
            typeof(string),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(decimal),
            typeof(double),
            typeof(bool),
            typeof(DateTime)
        };

        protected DbContext(string connectionString)
        {
            this.connection = new DatabaseConnection(connectionString);

            this.dbSetProperties = DiscoverDbSets();

            using (new ConnectionManager(this.connection))
            {
                this.InitializeDbSets();
            }

            this.MapAllRelations();
        }

        public void SaveChanges()
        {
            var dbSets = this.dbSetProperties
                .Select(pi => pi.Value.GetValue(this))
                .ToArray();

            foreach (IEnumerable<object> dbSet in dbSets)
            {
                var invalidEntities = dbSet
                    .Where(entity => !IsObjectValid(entity))
                    .ToArray();

                if (invalidEntities.Any())
                {
                    throw new InvalidOperationException(
                        $"{invalidEntities.Length} invalid Entities found in {dbSet.GetType().Name}.");
                }
            }

            using (new ConnectionManager(this.connection))
            {
                using (var transaction = this.connection.StartTransaction())
                {
                    foreach (IEnumerable<object> dbSet in dbSets)
                    {
                        var dbSetType = dbSet.GetType().GetGenericArguments().First();
                        // ? why .First

                        var persistMethod = typeof(DbContext)
                            .GetMethod("Persist", BindingFlags.Instance | BindingFlags.NonPublic)
                            .MakeGenericMethod(dbSetType);

                        try
                        {
                            persistMethod.Invoke(this, new object[] { dbSet });

                        }
                        catch (TargetInvocationException tie)
                        {
                            throw tie.InnerException;
                        }
                        catch (InvalidOperationException ioe)
                        {
                            transaction.Rollback();
                            throw;
                        }
                        catch (SqlException)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }

                    transaction.Commit();
                }
            }
        }

        private void Persist<TEntity>(DbSet<TEntity> dbSet)
            where TEntity : class, new()
        {
            var tableName = GetTableName(typeof(TEntity));

            var columns = this.connection.FetchColumnNames(tableName).ToArray();

            if (dbSet.ChangeTracker.Added.Any())
            {
                this.connection.InsertEntities(dbSet.ChangeTracker.Added, tableName, columns);
            }

            var modifiedEntities = dbSet.ChangeTracker.GetModifiedEntities(dbSet).ToArray();
            if (modifiedEntities.Any())
            {
                this.connection.UpdateEntities(modifiedEntities, tableName, columns);
            }

            if (dbSet.ChangeTracker.Removed.Any())
            {
                this.connection.DeleteEntities(dbSet.ChangeTracker.Removed, tableName, columns);
            }

        }

        private string GetTableName(Type tableType)
        {
            string tableName = string.Empty;

            try
            {
                tableName = ((TableAttribute)Attribute.GetCustomAttribute(tableType, typeof(TableAttribute))).Name;
            }
            catch (NullReferenceException)
            {
                tableName = this.dbSetProperties[tableType].Name;
            }

            return tableName;
        }

        private static bool IsObjectValid(object e)
        {
            var validationContext = new ValidationContext(e);
            var validationErrors = new List<ValidationResult>();

            var validationResult =
                Validator.TryValidateObject(e, validationContext, validationErrors, validateAllProperties: true);
            return validationResult;
        }

        private void MapAllRelations()
        {
            foreach (var dbSetProperty in this.dbSetProperties)
            {
                var dbSetType = dbSetProperty.Key;

                var mapRelationsGeneric = typeof(DbContext)
                    .GetMethod("MapRelations", BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(dbSetType);

                var dbSet = dbSetProperty.Value.GetValue(this);

                mapRelationsGeneric.Invoke(this, new[] {dbSet});
            }
        }

        private void MapRelations<TEntity>(DbSet<TEntity> dbSet)
            where TEntity : class, new()
        {
            var entityType = typeof(TEntity);

            this.MapNavigationProperties(dbSet);

            var collections = entityType
                .GetProperties()
                .Where(pi => pi.PropertyType.IsGenericType && 
                       pi.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                .ToArray();

            foreach (var collection in collections)
            {
                var collectionType = collection
                    .PropertyType
                    .GenericTypeArguments
                    .First();

                var mapCollectionMethod = typeof(DbContext)
                        .GetMethod("MapCollection", BindingFlags.Instance | BindingFlags.NonPublic)
                        .MakeGenericMethod(entityType, collectionType);

                mapCollectionMethod.Invoke(this, new object[] { dbSet, collection });
            }
        }

        private void MapNavigationProperties<TEntity>(DbSet<TEntity> dbSet) 
            where TEntity : class, new()
        {
            var entityType = typeof(TEntity);

            var foreignKeys = entityType.GetProperties()
                .Where(pi => pi.HasAttribute<ForeignKeyAttribute>())
                .ToArray();

            foreach (var foreignKey in foreignKeys)
            {
                var navigationPropertyName = foreignKey
                    .GetCustomAttribute<ForeignKeyAttribute>().Name;
                var navigationProperty = entityType.GetProperty(navigationPropertyName);

                var navigationDbSet = this.dbSetProperties[navigationProperty.PropertyType]
                    .GetValue(this);

                var navigationPrimaryKey = navigationProperty.PropertyType.GetProperties()
                    .First(pi => pi.HasAttribute<KeyAttribute>());

                foreach (var entity in dbSet)
                {
                    var foreignKeyValue = foreignKey.GetValue(entity);

                    var navigationPropertyValue = ((IEnumerable<object>)navigationDbSet)
                        .First(currentNavigationProperty => 
                        navigationPrimaryKey.GetValue(currentNavigationProperty).Equals(foreignKeyValue));

                    navigationProperty.SetValue(entity, navigationPropertyValue);
                }
            }
        }

        private void MapCollection<TDbSet, ICollection>(DbSet<TDbSet> dbSet, PropertyInfo collectionProperty) 
            where TDbSet : class, new() where ICollection : class, new()
        {
            var entityType = typeof(TDbSet);
            var collectionType = typeof(ICollection);

            var primaryKeys = collectionType.GetProperties()
                .Where(pi => pi.HasAttribute<KeyAttribute>())
                .ToArray();

            var primaryKey = primaryKeys.First();
            var foreignKey = entityType.GetProperties()
                .First(pi => pi.HasAttribute<KeyAttribute>());

            var isManyToMany = primaryKeys.Length >= 2;
            if (isManyToMany)
            {
                primaryKey = collectionType.GetProperties()
                    .First(pi => collectionType
                                    .GetProperty(pi.GetCustomAttribute<ForeignKeyAttribute>().Name)
                                    .PropertyType == entityType);
            }

            var navigationDbSet = (DbSet<ICollection>)this.dbSetProperties[collectionType].GetValue(this);

            foreach (var entity in dbSet)
            {
                var primaryKeyValue = foreignKey.GetValue(entity);

                var navigationEntities = navigationDbSet
                    .Where(navigationEntity => primaryKey.GetValue(navigationEntity)
                    .Equals(primaryKeyValue))
                    .ToArray();

                ReflectionHelper.ReplaceBackingField(entity, collectionProperty.Name, navigationEntities);
            }
        }

        private void InitializeDbSets()
        {
            foreach (var dbSet in this.dbSetProperties)
            {
                var dbSetType = dbSet.Key;
                var dbSetProperty = dbSet.Value;

                var populateDbSetGeneric = typeof(DbContext)
                    .GetMethod("PopulateDbSet", BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(dbSetType);

                populateDbSetGeneric.Invoke(this, new object[] { dbSetProperty });
            }
        }

        private void PopulateDbSet<TEntity>(PropertyInfo dbSet)
            where TEntity : class, new()
        {
            var entities = LoadTableEntities<TEntity>();

            var dbSetInstrance = new DbSet<TEntity>(entities);
            ReflectionHelper.ReplaceBackingField(this, dbSet.Name, dbSetInstrance);
        }

        private IEnumerable<TEntity> LoadTableEntities<TEntity>() 
            where TEntity : class
        {
            var table = typeof(TEntity);

            var columns = GetEntityColumnNames(table);

            var tableName = GetTableName(table);

            var fetchedRows = this.connection.FetchResultSet<TEntity>(tableName, columns)
                .ToArray();

            return fetchedRows;
        }

        private string[] GetEntityColumnNames(Type table)
        {
            var tableName = this.GetTableName(table);
            var dbColumns = this.connection.FetchColumnNames(tableName);

            var columns = table.GetProperties()
                .Where(pi => dbColumns.Contains(pi.Name) &&
                        !pi.HasAttribute<NotMappedAttribute>() &&
                        AllowedSqlTypes.Contains(pi.PropertyType))
                .Select(pi => pi.Name)
                .ToArray();

            return columns;

        }

        private Dictionary<Type, PropertyInfo> DiscoverDbSets()
        {
            var dbSets = this.GetType()
                .GetProperties()
                .Where(pi => pi.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .ToDictionary(pi => pi.PropertyType.GetGenericArguments().First(), pi => pi);

            return dbSets;
        }
    }
}