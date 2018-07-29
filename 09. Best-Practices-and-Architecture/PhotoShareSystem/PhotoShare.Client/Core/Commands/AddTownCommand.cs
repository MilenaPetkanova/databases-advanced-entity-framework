namespace PhotoShare.Client.Core.Commands
{
    using System;

    using Contracts;
    using Services.Contracts;

    public class AddTownCommand : ICommand
    {
        private const string SUCCESSFULLY_ADDED = "Town {0} was added successfully.";
        private const string ALREADY_ADDED = "Town {0} was already added.";

        private readonly ITownService townService;

        public AddTownCommand(ITownService townService)
        {
            this.townService = townService;
        }

        // AddTown <townName> <countryName>
        // Adds new  town. Town names must be unique.
        public string Execute(string[] data)
        {
            string townName = data[0];
            string country = data[1];

            this.CheckIfTownAlreadyExists(townName);

            this.townService.Add(townName, country);

            return string.Format(SUCCESSFULLY_ADDED, townName);
        }

        private void CheckIfTownAlreadyExists(string townName)
        {
            var townExists = this.townService.Exists(townName);

            if (townExists)
            {
                throw new ArgumentException(string.Format(SUCCESSFULLY_ADDED, townName));
            }
        }
    }
}
