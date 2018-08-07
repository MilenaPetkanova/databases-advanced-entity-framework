namespace FastFood.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Linq;
    using System.IO;
    using System.Xml.Serialization;

    using Newtonsoft.Json;

    using FastFood.Data;
    using FastFood.DataProcessor.Dto.Import;
    using FastFood.Models;
    using FastFood.Models.Enums;

    public static class Deserializer
	{
		private const string FailureMessage = "Invalid data format.";
		private const string SuccessMessage = "Record {0} successfully imported.";

		public static string ImportEmployees(FastFoodDbContext context, string jsonString)
		{
            var sb = new StringBuilder();

            var deserializedEmployeeDTOs = JsonConvert.DeserializeObject<EmployeeDTO[]>(jsonString);

            var employees = new List<Employee>();

            foreach (var employeeDTO in deserializedEmployeeDTOs)
            {
                if (!IsValid(employeeDTO))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var position = context.Positions.SingleOrDefault(p => p.Name.Equals(employeeDTO.Position));

                if (position == null)
                {
                    position = new Position
                    {
                        Name = employeeDTO.Position,
                    };

                    context.Positions.Add(position);
                    context.SaveChanges();
                }

                var employee = new Employee
                {
                    Name = employeeDTO.Name,
                    Age = employeeDTO.Age,
                    Position = position
                };

                employees.Add(employee);

                sb.AppendLine(String.Format(SuccessMessage, employeeDTO.Name));
            }

            context.AddRange(employees);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
		}

		public static string ImportItems(FastFoodDbContext context, string jsonString)
		{
            var sb = new StringBuilder();

            var deserializedItemDTOs = JsonConvert.DeserializeObject<ItemDTO[]>(jsonString);

            var items = new List<Item>();

            foreach (var itemDTO in deserializedItemDTOs)
            {
                if (!IsValid(itemDTO))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var itemExists = items.Any(i => i.Name.Equals(itemDTO.Name));
                if (itemExists)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var category = context.Categories.SingleOrDefault(c => c.Name.Equals(itemDTO.Category));
                if (category == null)
                {
                    category = new Category
                    {
                        Name = itemDTO.Category
                    };

                    context.Categories.Add(category);
                    context.SaveChanges();
                }
                
                var item = new Item
                {
                    Name = itemDTO.Name,
                    Category = category,
                    Price = itemDTO.Price
                };

                items.Add(item);

                sb.AppendLine(String.Format(SuccessMessage, itemDTO.Name));
            }

            context.Items.AddRange(items);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

		public static string ImportOrders(FastFoodDbContext context, string xmlString)
		{
            var sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(OrderDTO[]), new XmlRootAttribute("Orders"));
            var deserializedOrderDTOs = (OrderDTO[])serializer.Deserialize(new StringReader(xmlString));

            foreach (var orderDTO in deserializedOrderDTOs)
            {
                var employee = context.Employees.SingleOrDefault(e => e.Name.Equals(orderDTO.Employee));
                var allItemsExist = CheckIfAllItemsExist(context, orderDTO.Items);

                if (!IsValid(orderDTO) || employee == null || !allItemsExist)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var dateTime = DateTime.ParseExact(orderDTO.DateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                var type = Enum.TryParse(orderDTO.Type, out OrderType orderType) ? orderType : OrderType.ForHere;

                var order = new Order
                {
                    Customer = orderDTO.Customer,
                    Employee = employee,
                    DateTime = dateTime,
                    Type = type
                };

                context.Orders.Add(order);
                context.SaveChanges();

                var orderItems = new List<OrderItem>();

                foreach (var orderItemDTO in orderDTO.Items)
                {
                    var item = context.Items.SingleOrDefault(i => i.Name.Equals(orderItemDTO.Name));

                    var orderItem = new OrderItem
                    {
                        Item = item,
                        Order = order,
                        Quantity = orderItemDTO.Quantity
                    };

                    orderItems.Add(orderItem);
                }

                context.OrderItems.AddRange(orderItems);
                context.SaveChanges();

                sb.AppendLine($"Order for {orderDTO.Customer} on {orderDTO.DateTime} added");
            }

            var result = sb.ToString().Trim();
            return result;
        }

        private static bool CheckIfAllItemsExist(FastFoodDbContext context, OrderItemDTO[] items)
        {
            foreach (var item in items)
            {
                var existingItem = context.Items.SingleOrDefault(i => i.Name.Equals(item.Name));

                if (existingItem == null)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsValid(object obj)
        {
            var validatonContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

            return System.ComponentModel.DataAnnotations.Validator.TryValidateObject(obj, validatonContext, validationResults, true);
        }
    }
}