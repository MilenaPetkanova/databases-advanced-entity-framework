namespace P01_BillsPaymentSystem.App
{
    using P01_BillsPaymentSystem.Data;
    using P01_BillsPaymentSystem.Data.Models;
    using System;
    using System.Globalization;
    using System.Linq;

    public class Startup
    {
        public static void Main(string[] args)
        {
            var dbContext = new BillsPaymentSystemContext();

            using (dbContext)
            {
                Seed(dbContext);

                RetrieveUserPaymentsByUserId(dbContext);

                PayBills(dbContext);
            }
        }

        private static void PayBills(BillsPaymentSystemContext dbContext)
        {
            var userIdInput = int.Parse(Console.ReadLine());
            var amount = decimal.Parse(Console.ReadLine());

            var user = dbContext.Users
                .Where(u => u.UserId.Equals(userIdInput))
                .Select(u => new
                {
                    Name = u.FirstName + " " + u.LastName,
                    BankAccounts = u.UserPayments
                        .Where(up => up.PaymentType.Equals(PaymentType.BankAccount))
                        .Select(up => up.BankAccount)
                        .OrderBy(up => up.BankAccountId),
                    CreditCards = u.UserPayments
                        .Where(up => up.PaymentType.Equals(PaymentType.CreditCard))
                        .Select(up => up.CreditCard)
                        .OrderBy(up => up.CreditCardId)
                })
                .FirstOrDefault();

            var bankAccounts = user.BankAccounts.ToArray();
            var creditCards = user.CreditCards.ToArray();

            var totalMoney = bankAccounts.Sum(ba => ba.Balance) + creditCards.Sum(cc => cc.LimitLeft);

            if (amount > totalMoney)
            {
                Console.WriteLine("Insufficient funds!");
                return;
            }

            foreach (var bankAccount in bankAccounts)
            {
                if (amount >= bankAccount.Balance)
                {
                    amount -= bankAccount.Balance;
                    bankAccount.Withdraw(bankAccount.Balance);
                }
                else
                {
                    bankAccount.Withdraw(amount);
                    amount = 0;
                    break;
                }
            }

            foreach (var creditCard in creditCards)
            {
                if (amount >= creditCard.LimitLeft)
                {
                    amount -= creditCard.LimitLeft;
                    creditCard.Withdraw(creditCard.LimitLeft);
                }
                else
                {
                    creditCard.Withdraw(amount);
                    amount = 0;
                    break;
                }
            }
        }

        private static void RetrieveUserPaymentsByUserId(BillsPaymentSystemContext dbContext)
        {
            var userIdInput = int.Parse(Console.ReadLine());

            var user = dbContext.Users
                .Where(u => u.UserId.Equals(userIdInput))
                .Select(u => new
                {
                    Name = u.FirstName + u.LastName,
                    CreditCards = u.UserPayments
                        .Where(up => up.PaymentType.Equals(PaymentType.CreditCard))
                        .Select(up => up.CreditCard),
                    BankAccounts = u.UserPayments
                        .Where(up => up.PaymentType.Equals(PaymentType.BankAccount))
                        .Select(up => up.BankAccount)
                }).FirstOrDefault();

            if (user == null)
            {
                Console.WriteLine($"User with id {userIdInput} not found!");
                return;
            }

            Console.WriteLine($"User: {user.Name}");

            if (user.BankAccounts.Any())
            {
                Console.WriteLine("Bank Accounts:");
                foreach (var bankAccount in user.BankAccounts)
                {
                    Console.WriteLine($"-- ID: {bankAccount.BankAccountId}");
                    Console.WriteLine($"--- Balance: {bankAccount.Balance}");
                    Console.WriteLine($"--- Bank: {bankAccount.BankName}");
                    Console.WriteLine($"--- SWIFT: {bankAccount.SWIFT}");
                }
            }

            if (user.CreditCards.Any())
            {
                Console.WriteLine("Credit Cards:");
                foreach (var creditCard in user.CreditCards)
                {
                    Console.WriteLine($"-- ID: {creditCard.CreditCardId}");
                    Console.WriteLine($"--- Limit: {creditCard.Limit}");
                    Console.WriteLine($"--- Money Owed: {creditCard.MoneyOwed}");
                    Console.WriteLine($"--- Limit Left: {creditCard.LimitLeft}");
                    Console.WriteLine($"--- Expiration Date: {creditCard.ExpirationDate}");
                }
            }
        }

        private static void Seed(BillsPaymentSystemContext context)
        {
            var users = SeedUsers();
            var creditCards = SeedCreditCards();
            var bankAccounts = SeedBankAccounts();

            var userPayments = SeedUserPayments(users, creditCards, bankAccounts);

            context.Users.AddRange(users);
            context.CreditCards.AddRange(creditCards);
            context.BankAccounts.AddRange(bankAccounts);
            context.UserPayments.AddRange(userPayments);

            context.SaveChanges();
        }

        private static UserPayment[] SeedUserPayments(User[] users, CreditCard[] creditCards, BankAccount[] bankAccounts)
        {
            var paymentMethod1 = new UserPayment(users[0], PaymentType.CreditCard, creditCards[0]);
            var paymentMethod2 = new UserPayment(users[0], PaymentType.BankAccount, bankAccounts[0]);
            var paymentMethod3 = new UserPayment(users[0], PaymentType.BankAccount, bankAccounts[1]);

            //// Test Index -> the unique combination of UserId, BankAccountId and CreditCardId
            //var paymentMethod_testIndex = new UserPayment(users[0], PaymentType.CreditCard, creditCards[0]);

            //// Test Check Constraint -> always one of BankAccountId and CreditCardId is null and the other one is not 
            //paymentMethod1.BankAccount = bankAccounts[0];

            var paymentMethods = new UserPayment[] { paymentMethod1, paymentMethod2, paymentMethod3 };

            return paymentMethods;
        }

        private static BankAccount[] SeedBankAccounts()
        {
            var bankAccount1 = new BankAccount(1000m, "TestBankName1", "TestSWIFT1");
            var bankAccount2 = new BankAccount(1000000m, "TestBankName2", "TestSWIFT2");

            var banksAccounts = new BankAccount[] { bankAccount1, bankAccount2 };

            return banksAccounts;
        }

        private static CreditCard[] SeedCreditCards()
        {
            var creditCard1 = new CreditCard(100, 50.01m, DateTime.ParseExact("Mon 16 Jun 8:30 AM 2008", "ddd dd MMM h:mm tt yyyy", CultureInfo.InvariantCulture));
            var creditCard2 = new CreditCard(1000, 500.01m, DateTime.ParseExact("Mon 16 Jun 8:30 AM 2008", "ddd dd MMM h:mm tt yyyy", CultureInfo.InvariantCulture));
            
            var creditCards = new CreditCard[] { creditCard1, creditCard2 };

            return creditCards;
        }

        private static User[] SeedUsers()
        {
            var user1 = new User();
            user1.FirstName = "TestFirstName1";
            user1.LastName = "TestLastName1";
            user1.Email = "TestEmail1";
            user1.Password = "TestPassword1";

            var users = new User[] { user1 };

            return users;
        }
    }
}
