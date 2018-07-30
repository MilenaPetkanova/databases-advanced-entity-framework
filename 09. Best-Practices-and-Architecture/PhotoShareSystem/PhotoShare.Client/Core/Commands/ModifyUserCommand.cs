namespace PhotoShare.Client.Core.Commands
{
    using System;
    using System.Text.RegularExpressions;

    using Contracts;
    using PhotoShare.Services.Contracts;
    using PhotoShare.Client.Core.Dtos;

    public class ModifyUserCommand : ICommand
    {
        private const string SUCCESSFULLY_MODIFIED = "User {0} {1} is {2}.";
        private const string INVALID_USER = "User {0} not found.";
        private const string PROPERTY_NOT_FOUND = "Property {0} not supported.";
        private const string INVALID_VALUE = "Value {0} not valid.\n{1}";
        private const string INVALID_CREDENTIALS = "Invalid credentials.";

        private readonly IUserSessionService userSessionService;
        private readonly IUserService userService;
        private readonly ITownService townService;

        public ModifyUserCommand(IUserSessionService userSessionService, IUserService userService, ITownService townService)
        {
            this.userSessionService = userSessionService;
            this.userService = userService;
            this.townService = townService;
        }

        // ModifyUser <username> <property> <new value>
        // Modifies current user.
        // For example:
        // ModifyUser <username> Password <NewPassword>
        // ModifyUser <username> BornTown <newBornTownName>
        // ModifyUser <username> CurrentTown <newCurrentTownName>
        // !!! Cannot change username
        public string Execute(string[] data)
        {
            var username = data[0];
            var property = data[1];
            var newValue = data[2];

            this.ValidateUser(username);
            this.CheckIfLoggedIn();

            var userId = this.userService.ByUsername<UserDto>(username).Id;
            
            switch (property)
            {
                case "Password":
                    this.ValidatePassword(newValue);
                    this.userService.ChangePassword(userId, newValue);
                    break;
                case "BornTown":
                    this.ValidateTown(newValue);
                    var bornTownId = this.townService.ByName<TownDto>(newValue).Id;
                    this.userService.SetBornTown(userId, bornTownId);
                    break;
                case "CurrentTown":
                    this.ValidateTown(newValue);
                    var currentTownId = this.townService.ByName<TownDto>(newValue).Id;
                    this.userService.SetCurrentTown(userId, currentTownId);
                    break;
                default:
                    throw new ArgumentException(string.Format(PROPERTY_NOT_FOUND, property));
            }

            return string.Format(SUCCESSFULLY_MODIFIED, username, property, newValue);
        }

        private void ValidateUser(string username)
        {
            if (!this.userService.Exists(username))
            {
                throw new ArgumentException(string.Format(INVALID_USER, username));
            }
        }

        private void CheckIfLoggedIn()
        {
            if (!this.userSessionService.IsLoggedIn())
            {
                throw new InvalidOperationException(INVALID_CREDENTIALS);
            }
        }

        private void ValidateTown(string townName)
        {
            if (!this.townService.Exists(townName))
            {
                var detailedMessage = $"Town {townName} not found.";

                throw new ArgumentException(string.Format(INVALID_VALUE, townName, detailedMessage));
            }
        }

        private void ValidatePassword(string newPassword)
        {
            var pattern = @"^(?=.*[a-z])(?=.*[0-9]).+$";

            if (!Regex.IsMatch(newPassword, pattern))
            {
                var detailedMessage = "Invalid Password.";

                throw new ArgumentException(string.Format(INVALID_VALUE, newPassword, detailedMessage));

            }
        }
    }
}
