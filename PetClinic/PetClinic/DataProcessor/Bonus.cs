namespace PetClinic.DataProcessor
{
    using PetClinic.Data;
    using System.Linq;

    public class Bonus
    {
        public static string UpdateVetProfession(PetClinicContext context, string phoneNumber, string newProfession)
        {
            var result = string.Empty;

            var vet = context.Vets.SingleOrDefault(v => v.PhoneNumber.Equals(phoneNumber));

            if (vet == null)
            {
                result = $"Vet with phone number {phoneNumber} not found!";
                return result;
            }

            var oldProfession = vet.Profession;

            vet.Profession = newProfession;
            context.SaveChanges();

            result = $"{vet.Name}'s profession updated from {oldProfession} to {newProfession}.";
            return result;
        }
    }
}
