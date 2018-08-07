namespace FastFood.DataProcessor
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using System.Xml;

    using Newtonsoft.Json;

    using FastFood.Models.Enums;
    using FastFood.Data;
    using FastFood.DataProcessor.Dto.Export;
    
    public class Serializer
    {
        public static string ExportOrdersByEmployee(FastFoodDbContext context, string employeeName, string orderType)
        {
            var type = Enum.Parse<OrderType>(orderType);

            var ordersByEmployeeDTOs = context.Employees.ToArray()
                .Where(e => e.Name.Equals(employeeName))
                .Select(e => new EmployeeOrdersDTO
                {
                    Name = e.Name,
                    Orders = e.Orders
                        .Where(o => o.Type.Equals(type))
                        .Select(o => new OrderDTO
                        {
                            Customer = o.Customer,
                            Items = o.OrderItems.Select(oi => new ItemDTO
                            {
                                Name = oi.Item.Name,
                                Price = Math.Round(oi.Item.Price, 2),
                                Quantity = oi.Quantity
                            })
                            .ToArray(),
                            TotalPrice = o.OrderItems.Sum(oi => oi.Quantity * oi.Item.Price)
                        })
                        .OrderByDescending(o => o.TotalPrice)
                        .ThenByDescending(o => o.Items.Length)
                        .ToArray(),
                    TotalMade = e.Orders.Where(o => o.Type.Equals(type))
                                        .Sum(o => o.OrderItems
                                        .Sum(oi => oi.Quantity * oi.Item.Price))
                })
                .FirstOrDefault();

            var jsonSerializerSettings = GetDefaultNullValueHandling();

            var json = JsonConvert.SerializeObject(ordersByEmployeeDTOs, Newtonsoft.Json.Formatting.Indented, jsonSerializerSettings);

            return json;
        }

        public static string ExportCategoryStatistics(FastFoodDbContext context, string categoriesString)
        {
            var categoryNames = categoriesString.Split(',');

            var categoryStatistics = context.Items
                .Where(i => categoryNames.Any(c => c == i.Category.Name))
                .GroupBy(i => i.Category.Name)
                .Select(g => new Dto.Export.CategoryDTO
                {
                    Name = g.Key,
                    MostPopularItem = g.Select(i => new CategoryItemDTO
                    {
                        Name = i.Name,
                        TotalMade = i.OrderItems.Sum(oi => oi.Quantity * oi.Item.Price),
                        TimesSold = i.OrderItems.Sum(oi => oi.Quantity)
                    })
                    .OrderByDescending(i => i.TotalMade)
                    .ThenByDescending(i => i.TimesSold)
                    .First()
                })
                .OrderByDescending(dto => dto.MostPopularItem.TotalMade)
                .ThenByDescending(dto => dto.MostPopularItem.TimesSold)
                .ToArray();

            var sb = new StringBuilder();
            var xmlNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            var serializer = new XmlSerializer(typeof(CategoryDTO[]), new XmlRootAttribute("Categories"));
            serializer.Serialize(new StringWriter(sb), categoryStatistics, xmlNamespaces);

            var result = sb.ToString();
            return result;
        }

        private static JsonSerializerSettings GetDefaultNullValueHandling()
        {
            var settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            return settings;
        }
    }
}