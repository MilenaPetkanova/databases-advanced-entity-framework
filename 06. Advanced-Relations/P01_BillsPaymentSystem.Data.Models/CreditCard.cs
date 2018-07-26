namespace P01_BillsPaymentSystem.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public class CreditCard
    {
        private decimal limit;
        private decimal moneyOwed;

        public CreditCard()
        {
        }

        public CreditCard(decimal limit, decimal moneyOwed, DateTime expirationDate)
        {
            this.Limit = limit;
            this.MoneyOwed = moneyOwed;
            this.ExpirationDate = expirationDate;
        }

        public int CreditCardId { get; set; }

        public decimal Limit
        {
            get => this.limit;
            private set => this.limit = value;
        }

        public decimal MoneyOwed
        {
            get => this.moneyOwed;
            private set
            {
                if (value > this.Limit)
                {
                    throw new InvalidOperationException("The credit card limit is already reached.");
                }
                this.moneyOwed = value;
            } 
        }

        [NotMapped]
        public decimal LimitLeft => this.Limit - this.MoneyOwed;

        public DateTime ExpirationDate { get; set; }

        [NotMapped]
        public int UserPaymentId { get; set; }
        public UserPayment UserPayment { get; set; }

        public void Withdraw(decimal amount)
        {
            this.MoneyOwed += amount;
        }

        public void Deposit(decimal amount)
        {
            this.MoneyOwed -= amount;
        }
    }
}
