namespace P03_SalesDatabase.Data.Models
{
    using System.Collections.Generic;

    public class Store
    {
        public int StoreId { get; set; }

        public string Name { get; set; }

        public int MyProperty { get; set; }

        public ICollection<Sale> Sales { get; set; }
    }
}
