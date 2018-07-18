namespace P01_BillsPaymentSystem.Data.Models
{
    using System.Collections.Generic;

    public class User
    {
        public User()
        {
            this.UserPayments = new List<UserPayment>();
        }

        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public ICollection<UserPayment> UserPayments { get; set; }
    }
}
