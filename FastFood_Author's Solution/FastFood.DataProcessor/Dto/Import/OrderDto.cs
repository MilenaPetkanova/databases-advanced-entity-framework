using System;

namespace FastFood.DataProcessor.Dto.Import
{
	public class OrderDto
	{
		public string Customer { get; set; }

		public string Employee { get; set; }

		public DateTime DateTime { get; set; }

		public ItemDto[] Items { get; set; }
	}
}