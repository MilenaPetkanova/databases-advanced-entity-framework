namespace ProductShop.App
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    using AutoMapper;

    using ProductShop.Data;
    using ProductShop.Models;
    using ProductShop.App.Dtos.Import;
    using ProductShop.App.Dtos.Export;

    public class XmlProcessor
    {
        private const string PATH_TO_IMPORT_FOLDER = "Xmls/Import/";
        private const string PATH_TO_EXPORT_FOLDER = "Xmls/Export/";

        private ProductShopContext context;

        public XmlProcessor()
        {
            this.context = new ProductShopContext();
        }

        public void ImportData()
        {
            var config = new MapperConfiguration(cfg => 
            {
                cfg.AddProfile<ProductShopProfile>();
            });

            IMapper mapper = config.CreateMapper();

            this.ImportUsers(mapper);
            this.ImportProducts(mapper);
            this.ImportCategories(mapper);
            this.GenerateCategoryProducts(mapper);

        }

        public void ExportData()
        {
            this.ExportProductsInRange();
            this.ExportUsersSoldProducts();
            this.ExportCategoriesByProductsCount();
            this.ExportUsersAndProducts();
        }

        private void ImportUsers(IMapper mapper)
        {
            var xmlString = File.ReadAllText(PATH_TO_IMPORT_FOLDER + "users.xml");
            var serializer = new XmlSerializer(typeof(UserImportDto[]), new XmlRootAttribute("users"));
            var deserializedUserDtos = (UserImportDto[])serializer.Deserialize(new StringReader(xmlString));

            var users = new List<User>();

            foreach (var userDto in deserializedUserDtos)
            {
                if (!IsValid(userDto))
                {
                    continue;
                }

                var user = mapper.Map<User>(userDto);
                users.Add(user);
            }

            this.context.Users.AddRange(users);
            this.context.SaveChanges();
        }

        private void ImportProducts(IMapper mapper)
        {
            var xmlString = File.ReadAllText(PATH_TO_IMPORT_FOLDER + "products.xml");
            var serializer = new XmlSerializer(typeof(ProductImportDto[]), new XmlRootAttribute("products"));
            var deserializedProductDtos = (ProductImportDto[])serializer.Deserialize(new StringReader(xmlString));

            var random = new Random();
            var usersCount = this.context.Users.Select(u => u.Id).Count();

            var products = new List<Product>();

            for (int i = 0; i < deserializedProductDtos.Length; i++)
            {
                var productDto = deserializedProductDtos[i];

                if (IsValid(productDto))
                {
                    continue;
                }

                var product = mapper.Map<Product>(productDto);

                product.SellerId = random.Next(1, usersCount);

                if (i % 10 != 0)
                {
                    product.BuyerId = random.Next(1, usersCount);
                }

                products.Add(product);
            }

            this.context.Products.AddRange(products);
            this.context.SaveChanges();
        }

        private void ImportCategories(IMapper mapper)
        {
            var xmlString = File.ReadAllText(PATH_TO_IMPORT_FOLDER + "categories.xml");
            var serializer = new XmlSerializer(typeof(CategoryImportDto[]), new XmlRootAttribute("categories"));
            var deserializedProductDtos = (CategoryImportDto[]) serializer.Deserialize(new StringReader(xmlString));

            var categories = new List<Category>();

            foreach (var categoryDto in deserializedProductDtos)
            {
                if (IsValid(categoryDto))
                {
                    var category = mapper.Map<Category>(categoryDto);
                    categories.Add(category);
                }
            }

            this.context.AddRange(categories);
            this.context.SaveChanges();
        }

        private void GenerateCategoryProducts(IMapper mapper)
        {
            var random = new Random();
            var categoriesCount = this.context.Categories.Select(c => c.Id).Count();

            var categoriesProducts = new List<CategoryProduct>();

            foreach (var product in this.context.Products)
            {
                var categoryProduct = new CategoryProduct
                {
                    ProductId = product.Id,
                    CategoryId = random.Next(1, categoriesCount)
                };

                categoriesProducts.Add(categoryProduct);
            }

            this.context.CategoryProducts.AddRange(categoriesProducts);
            this.context.SaveChanges();
        }

        private bool IsValid(object obj)
        {
            var validatonContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

            return System.ComponentModel.DataAnnotations.Validator.TryValidateObject(obj, validatonContext, validationResults, true);
        }

        private void ExportProductsInRange()
        {
            var productDtos = this.context.Products
                .Where(p => p.Price >= 1000 && p.Price <= 2000 && p.Buyer != null)
                .OrderBy(p => p.Price)
                .Select(p => new ProductInRangeDTO
                {
                    Name = p.Name,
                    Price = p.Price,
                    BuyerFullName = p.Buyer.FirstName + " " + p.Buyer.LastName ?? p.Buyer.LastName
                })
                .ToArray();

            var sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(ProductInRangeDTO[]), new XmlRootAttribute("products"));
            serializer.Serialize(new StringWriter(sb), productDtos);

            File.WriteAllText(PATH_TO_EXPORT_FOLDER + "products.xml", sb.ToString());
        }

        private void ExportUsersSoldProducts()
        {
            var usersSoldProductsDtos = this.context.Users
                .Where(u => u.ProductsSold.Count > 0)
                .Select(u => new UserSoldProductsDTO
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold.Select(p => new SoldProductElementsDTO
                    {
                        Name = p.Name,
                        Price = p.Price
                    })
                    .ToArray()
                })
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToArray();

            var sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(UserSoldProductsDTO[]), new XmlRootAttribute("users"));
            serializer.Serialize(new StringWriter(sb), usersSoldProductsDtos);

            File.WriteAllText(PATH_TO_EXPORT_FOLDER + "users-sold-products.xml", sb.ToString());
        }

        private void ExportCategoriesByProductsCount()
        {
            var categoriesByProductsDtos = this.context.Categories
                .Select(c => new CategoryProductsInfoDTO
                {
                    Name = c.Name,
                    ProductsCount = c.CategoryProducts.Count,
                    AveragePrice = c.CategoryProducts.Select(cp => cp.Product.Price).DefaultIfEmpty(0).Average(),
                    TotalRevenue = c.CategoryProducts.Sum(p => p.Product.Price)
                })
                .OrderBy(cp => cp.ProductsCount)
                .ToArray();

            var sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(CategoryProductsInfoDTO[]), new XmlRootAttribute("categories"));
            serializer.Serialize(new StringWriter(sb), categoriesByProductsDtos);

            File.WriteAllText(PATH_TO_EXPORT_FOLDER + "categories-by-products.xml", sb.ToString());
        }

        private void ExportUsersAndProducts()
        {
            var usersCollectionDto = new UsersCollectionDTO
            {
                Count = this.context.Users.Count(),
                UserDTOs = this.context.Users
                    .Where(u => u.ProductsSold.Count > 0)
                    .OrderByDescending(u => u.ProductsSold.Count)
                    .Select(u => new UserDTO
                    {
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        AgeValue = u.Age.ToString(),
                        SoldProducts = new SoldProductsCollectionDTO
                        {
                            Count = u.ProductsSold.Count,
                            SoldProductDTOs = u.ProductsSold.Select(sp => new SoldProductAttributesDTO
                            {
                                Name = sp.Name,
                                Price = sp.Price
                            })
                            .ToArray()
                        }
                    })
                    .ToArray()
            };

            var sb = new StringBuilder();
            var xmlNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            var serializer = new XmlSerializer(typeof(UsersCollectionDTO));
            serializer.Serialize(new StringWriter(sb), usersCollectionDto, xmlNamespaces);

            File.WriteAllText(PATH_TO_EXPORT_FOLDER + "users-and-products.xml", sb.ToString()); 
        }
    }
}