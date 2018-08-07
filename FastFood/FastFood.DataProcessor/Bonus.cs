namespace FastFood.DataProcessor
{
    using System.Linq;

    using FastFood.Data;

    public static class Bonus
    {
	    public static string UpdatePrice(FastFoodDbContext context, string itemName, decimal newPrice)
	    {
            var result = string.Empty;

            var item = context.Items.FirstOrDefault(i => i.Name.Equals(itemName));
            if (item == null)
            {
                result = $"Item {itemName} not found!";
                return result;
            }

            var oldPrice = item.Price;

            item.Price = newPrice;
            context.SaveChanges();

            result = $"{itemName} Price updated from ${oldPrice} to ${newPrice}";

            return result;
	    }
    }
}
