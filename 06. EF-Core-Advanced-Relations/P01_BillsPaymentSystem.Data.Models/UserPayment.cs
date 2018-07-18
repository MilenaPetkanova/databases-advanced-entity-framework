namespace P01_BillsPaymentSystem.Data.Models
{
    public class UserPayment
    {
        public UserPayment()
        {
        }

        public UserPayment(User user, PaymentType paymentType, CreditCard creditCard)
        {
            this.User = user;
            this.PaymentType = paymentType;
            this.CreditCard = creditCard;
        }

        public UserPayment(User user, PaymentType paymentType, BankAccount bankAccount)
        {
            this.User = user;
            this.PaymentType = paymentType;
            this.BankAccount = bankAccount;
        }

        public int Id { get; set; }

        public PaymentType PaymentType { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int? BankAccountId { get; set; }
        public BankAccount BankAccount { get; set; }

        public int? CreditCardId { get; set; }
        public CreditCard CreditCard { get; set; }
    }
}
