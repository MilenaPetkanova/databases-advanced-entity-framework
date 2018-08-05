namespace ProductShop.App
{
    using Newtonsoft.Json;
    using ProductShop.App.Dto;
    using ProductShop.Data;
    using ProductShop.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class JsonProcessor
    {
        private const string PATH_TO_IMPORT = "Json/ImportedFiles/";
        private const string PATH_TO_EXPORT = "Json/ExportResults/";

        private ProductShopContext context;

        public JsonProcessor(ProductShopContext context)
        {
            this.context = context;
        }

        public void ImportData()
        {
            this.ImportUsers();
            this.ImportProducts();
            this.ImportCategories();
            this.GenerateCategoryProducts();
        }

        internal void EmportData()
        {
            //this.ExportProductsInRange();
            //this.ExportSuccessfullySoldProducts();
            //this.ExportCategoriesByProductsCount();
            this.ExportUsersAndProducts();
        }

        private void ImportUsers()
        {
            var jsonString = File.ReadAllText(PATH_TO_IMPORT + "users.json");

            var deserializedUsers = JsonConvert.DeserializeObject<User[]>(jsonString);

            this.context.AddRange(deserializedUsers);

            this.context.SaveChanges();
        }

        private void ImportProducts()
        {
            var jsonString = File.ReadAllText(PATH_TO_IMPORT + "products.json");

            var deserializedProducts = JsonConvert.DeserializeObject<Product[]>(jsonString);

            var products = new List<Product>();

            var random = new Random();
            var usersCount = this.context.Users.Select(u => u.Id).Count();
            var nullBuyerJump = random.Next(1, deserializedProducts.Length);

            for (int i = 0; i < deserializedProducts.Length; i++)
            {
                var product = deserializedProducts[i];

                var sellerId = random.Next(1, usersCount);
                product.SellerId = sellerId;

                if (i % nullBuyerJump != 0)
                {
                    var buyerId = random.Next(1, usersCount);
                    product.BuyerId = buyerId;
                }

                products.Add(product);
            }

            this.context.Products.AddRange(products);
            this.context.SaveChanges();
        }

        private void ImportCategories()
        {
            var jsonString = File.ReadAllText(PATH_TO_IMPORT + "categories.json");

            var deserializedCategories = JsonConvert.DeserializeObject<Category[]>(jsonString);

            this.context.Categories.AddRange(deserializedCategories);

            this.context.SaveChanges();
        }

        private void GenerateCategoryProducts()
        {
            var categoryProducts = new List<CategoryProduct>();

            var random = new Random();
            var categoriesCount = this.context.Categories.Count();

            foreach (var product in this.context.Products)
            {
                var categoryProduct = new CategoryProduct
                {
                    ProductId = product.Id,
                    CategoryId = random.Next(1, categoriesCount)
                };

                categoryProducts.Add(categoryProduct);
            }

            this.context.CategoryProducts.AddRange(categoryProducts);
            this.context.SaveChanges();
        }

        private void ExportProductsInRange()
        {
            var productInRangeDTOs = this.context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new ProductInRangeDTO
                {
                    Name = p.Name,
                    Price = p.Price,
                    Seller = p.Seller.LastName == null ? p.Seller.FirstName : $"{p.Seller.FirstName} {p.Seller.LastName}"
                })
                .ToArray();

            var jsonSerializerSettings = this.GetDefaultNullValueHandling();

            var jsonString = JsonConvert.SerializeObject(productInRangeDTOs, Formatting.Indented, jsonSerializerSettings);

            File.WriteAllText(PATH_TO_EXPORT + "products-in-range.json", jsonString);

        }

        private void ExportSuccessfullySoldProducts()
        {
            var userSoldProductsDTOs = this.context.Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new UserSoldProductsDTO
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    ProductsSold = u.ProductsSold.Select(p => new SoldProductAndBuyerInfoDTO
                    {
                        Name = p.Name,
                        Price = p.Price,
                        BuyerFirstName = p.Buyer.FirstName,
                        BuyerLastName = p.Buyer.LastName
                    })
                    .ToArray()
                })
                .ToArray();

            var jsonSerializerSettings = this.GetDefaultNullValueHandling();

            var jsonString = JsonConvert.SerializeObject(userSoldProductsDTOs, Formatting.Indented, jsonSerializerSettings);

            File.WriteAllText(PATH_TO_EXPORT + "users-sold-products.json", jsonString);
        }

        private void ExportCategoriesByProductsCount()
        {
            var categoriesInfoDTOs = this.context.Categories
                .Select(c => new CategoryInfoDTO
                {
                    Name = c.Name,
                    ProductsCount = c.CategoryProducts.Count,
                    AveragePrice = c.CategoryProducts.Select(cp => cp.Product.Price).DefaultIfEmpty(0).Average(),
                    TotalRevenue = c.CategoryProducts.Sum(cp => cp.Product.Price)
                })
                .ToArray()
                .OrderByDescending(c => c.ProductsCount)
                .ToArray();

            var jsonSerializerSettings = this.GetDefaultNullValueHandling();

            var jsonString = JsonConvert.SerializeObject(categoriesInfoDTOs, Formatting.Indented, jsonSerializerSettings);

            File.WriteAllText(PATH_TO_EXPORT + "categories-by-products.json", jsonString);
        }

        private JsonSerializerSettings GetDefaultNullValueHandling()
        {
            var settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            return settings;
        }

        private void ExportUsersAndProducts()
        {
            var usersAndProductsDTOs = new UserCollectionDTO
                {
                    UsersCount = this.context.Users
                        .Where(u => u.ProductsSold.Count > 0)
                        .Count(),
                    Users = this.context.Users
                        .Select(u => new UserDTO
                        {
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Age = u.Age,
                            SoldProducts = new SoldProductCollectionDTO
                            {
                                Count = u.ProductsSold.Count,
                                Products = u.ProductsSold
                                    .Select(p => new SoldProductDTO
                                    {
                                        Name = p.Name,
                                        Price = p.Price
                                    })
                                    .ToArray()
                            }
                        })
                        .ToArray()
                };

            var jsonSerializerSettings = this.GetDefaultNullValueHandling();

            var jsonString = JsonConvert.SerializeObject(usersAndProductsDTOs, Formatting.Indented, jsonSerializerSettings);

            File.WriteAllText(PATH_TO_EXPORT + "users-and-products.json", jsonString);
        }
    }
}
