using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using FastFood.Data;
using FastFood.DataProcessor.Dto.Import;
using FastFood.Models;
using FastFood.Models.Enums;
using Newtonsoft.Json;

namespace FastFood.DataProcessor
{
	public static class Deserializer
	{
		private const string FailureMessage = "Invalid data format.";
		private const string SuccessMessage = "Record {0} successfully imported.";

		public static string ImportEmployees(FastFoodDbContext context, string jsonString)
		{
			var deserializedEmployees = JsonConvert.DeserializeObject<EmployeeDto[]>(jsonString);

			var validEmployees = new List<Employee>();

			var sb = new StringBuilder();

			foreach (var employeeDto in deserializedEmployees)
			{
				if (!IsValid(employeeDto))
				{
					sb.AppendLine(FailureMessage);
					continue;
				}

				var position = FindOrCreatePosition(context, employeeDto.Position);

				var employee = new Employee
				{
					Name = employeeDto.Name,
					Age = employeeDto.Age,
					Position = position
				};

				validEmployees.Add(employee);
				sb.AppendLine(string.Format(SuccessMessage, employee.Name));
			}

			context.Employees.AddRange(validEmployees);
			context.SaveChanges();

			var result = sb.ToString();
			return result;
		}

		public static string ImportItems(FastFoodDbContext context, string jsonString)
		{
			var deserializedItems = JsonConvert.DeserializeObject<ItemDto[]>(jsonString);

			var sb = new StringBuilder();

			var validItems = new List<Item>();
			foreach (var itemDto in deserializedItems)
			{
				if (!IsValid(itemDto) || validItems.Any(i => i.Name == itemDto.Name))
				{
					sb.AppendLine(FailureMessage);
					continue;
				}

				var category = FindOrCreateCategory(context, itemDto.Category);

				var item = new Item
				{
					Name = itemDto.Name,
					Price = itemDto.Price,
					Category = category
				};

				validItems.Add(item);
				sb.AppendLine(string.Format(SuccessMessage, item.Name));
			}

			context.Items.AddRange(validItems);
			context.SaveChanges();

			var result = sb.ToString();
			return result;
		}

		public static string ImportOrders(FastFoodDbContext context, string xmlString)
		{
			var xml = XDocument.Parse(xmlString);

			var deserializedOrders = xml.Element("Orders");

			var validOrders = new List<Order>();
			var sb = new StringBuilder();

			foreach (var orderElement in deserializedOrders.Elements())
			{
				var order = new Order();

				var customer = orderElement.Element("Customer").Value;
				order.Customer = customer;

				var employee = FindEmployee(context, orderElement.Element("Employee").Value);

				if (employee == null)
				{
					sb.AppendLine(FailureMessage);
					continue;
				}

				order.Employee = employee;

				var orderType = Enum.Parse<OrderType>(orderElement.Element("Type").Value);
				order.Type = orderType;

				var dateTime = DateTime.ParseExact(orderElement.Element("DateTime").Value, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
				order.DateTime = dateTime;

				var orderItems = new List<OrderItem>();

				var itemsElement = orderElement.Element("Items");
				foreach (var itemElement in itemsElement.Elements())
				{
					var item = FindItem(context, itemElement.Element("Name").Value);

					if (item == null)
					{
						sb.AppendLine(FailureMessage);
						continue;
					}

					var quantity = int.Parse(itemElement.Element("Quantity").Value);

					var orderItem = new OrderItem {Item = item, Quantity = quantity};
					orderItems.Add(orderItem);
				}

				order.OrderItems = orderItems;
				if (!IsValid(order))
				{
					sb.AppendLine(FailureMessage);
					continue;
				}

				validOrders.Add(order);
				sb.AppendLine(string.Format("Order for {0} on {1} added", 
					order.Customer, 
					order.DateTime.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)));
			}

			context.Orders.AddRange(validOrders);
			context.SaveChanges();

			var result = sb.ToString();

			return result;
		}

		private static Item FindItem(FastFoodDbContext context, string itemName)
		{
			var item = context.Items.SingleOrDefault(i => i.Name == itemName);
			return item;
		}

		private static Employee FindEmployee(FastFoodDbContext context, string employeeName)
		{
			var employee = context.Employees.SingleOrDefault(e => e.Name == employeeName);
			return employee;
		}

		private static Position FindOrCreatePosition(FastFoodDbContext context, string positionName)
		{
			var position = FindPosition(context, positionName) ?? CreatePosition(context, positionName);
			return position;
		}

		private static Position CreatePosition(FastFoodDbContext context, string positionName)
		{
			var position = new Position { Name = positionName };

			context.Positions.Add(position);
			context.SaveChanges();

			return position;
		}

		private static Position FindPosition(FastFoodDbContext context, string positionName)
		{
			var position = context.Positions.SingleOrDefault(p => p.Name == positionName);

			return position;
		}

		private static Category FindOrCreateCategory(FastFoodDbContext context, string categoryName)
		{
			var category = FindCategory(context, categoryName) ?? CreateCategory(context, categoryName);
			return category;
		}

		private static Category FindCategory(FastFoodDbContext context, string categoryName)
		{
			var category = context.Categories.SingleOrDefault(c => c.Name == categoryName);
			return category;
		}

		private static Category CreateCategory(FastFoodDbContext context, string categoryName)
		{
			var category = new Category {Name = categoryName};

			context.Categories.Add(category);
			context.SaveChanges();

			return category;
		}

		private static bool IsValid(object obj)
		{
			var validationContext = new ValidationContext(obj);
			var validationResults = new List<ValidationResult>();

			var isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);

			return isValid;
		}
	}
}