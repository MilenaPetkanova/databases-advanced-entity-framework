namespace P01_BillsPaymentSystem.Data.EntityConfig
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using P01_BillsPaymentSystem.Data.Models;

    public class UserPaymentConfig : IEntityTypeConfiguration<UserPayment>
    {
        public void Configure(EntityTypeBuilder<UserPayment> builder)
        {
            builder.HasKey(up => up.Id);

            builder.HasIndex(up => new { up.UserId, up.CreditCardId, up.BankAccountId })
                .IsUnique();

            builder.HasOne(up => up.User)
                .WithMany(u => u.UserPayments)
                .HasForeignKey(up => up.UserId);

            builder.HasOne(up => up.CreditCard)
                .WithOne(cc => cc.UserPayment)
                .HasForeignKey<UserPayment>(up => up.CreditCardId)
                .IsRequired(false);

            builder.HasOne(up => up.BankAccount)
                .WithOne(ba => ba.UserPayment)
                .HasForeignKey<UserPayment>(up => up.BankAccountId)
                .IsRequired(false); ;
        }
    }
}
