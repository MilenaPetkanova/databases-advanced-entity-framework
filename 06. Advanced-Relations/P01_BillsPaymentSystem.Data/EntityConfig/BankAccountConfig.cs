namespace P01_BillsPaymentSystem.Data.EntityConfig
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using P01_BillsPaymentSystem.Data.Models;

    public class BankAccountConfig : IEntityTypeConfiguration<BankAccount>
    {
        public void Configure(EntityTypeBuilder<BankAccount> builder)
        {
            builder.HasKey(ba => ba.BankAccountId);

            builder.Property(ba => ba.Balance)
                .IsRequired();

            builder.Property(ba => ba.BankName)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(50);

            builder.Property(ba => ba.SWIFT)
                .IsRequired()
                .IsUnicode(false)
                .HasMaxLength(20);

            builder.Ignore(ba => ba.UserPaymentId);
        }
    }
}
