namespace SoftJail.DataProcessor
{

    using Data;
    using System;
    using System.Linq;

    public class Bonus
    {
        public static string ReleasePrisoner(SoftJailDbContext context, int prisonerId)
        {
            var result = string.Empty;

            var prisoner = context.Prisoners.SingleOrDefault(p => p.Id == prisonerId);

            if (prisoner.ReleaseDate == null)
            {
                result = $"Prisoner {prisoner.FullName} is sentenced to life";
                return result;
            }

            prisoner.CellId = 0;
            prisoner.ReleaseDate = DateTime.Now;

            result = $"Prisoner {prisoner.FullName} released";
            return result;
        }
    }
}
