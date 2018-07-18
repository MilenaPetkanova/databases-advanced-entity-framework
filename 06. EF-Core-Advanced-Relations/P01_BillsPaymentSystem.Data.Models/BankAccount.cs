namespace P01_BillsPaymentSystem.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public class BankAccount
    {
        private decimal balance;

        public BankAccount()
        {
        }

        public BankAccount(decimal balance, string bankName, string swiftCode)
        {
            this.Balance = balance;
            this.BankName = bankName;
            this.SWIFT = swiftCode;
        }

        public int BankAccountId { get; set; }

        public decimal Balance
        {
            get => this.balance;
            private set
            {
                if (value < 0)
                {
                    throw new InvalidOperationException("No money left.");
                }
                this.balance = value;
            }
        }

        public string BankName { get; set; }

        public string SWIFT { get; set; }

        [NotMapped]
        public int UserPaymentId { get; set; }
        public UserPayment UserPayment { get; set; }

        public void Withdraw(decimal amount)
        {
            this.Balance -= amount;
        }

        public void Deposit(decimal amount)
        {
            this.Balance += amount;
        }
    }
}
