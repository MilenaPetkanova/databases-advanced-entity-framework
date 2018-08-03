namespace CarDealer.Models
{
    public class Sale
    {
        public int Id { get; set; }

        public decimal Discount { get; set; }

        public int Car_Id { get; set; }
        public Car Car { get; set; }

        public int Customer_Id { get; set; }
        public Customer Customer { get; set; }
    }
}
