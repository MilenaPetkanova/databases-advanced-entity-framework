namespace TeamBuilder.App.Core.Commands
{
    using System;
    using System.Text.RegularExpressions;
    using TeamBuilder.App.Core.Commands.Contracts;
    using TeamBuilder.App.Utilities;
    using TeamBuilder.Data;
    using TeamBuilder.Models;
    using TeamBuilder.Models.Enums;

    public class RegisterUserCommand : ICommand
    {
        // RegisterUser <username> <password> <repeat-password> <firstName> <lastName> <age> <gender>
        public string Execute(string[] inputArgs)
        {
            Check.CheckLength(7, inputArgs);

            var username = inputArgs[0];

            if (username.Length < Constants.MinUsernameLength || username.Length > Constants.MaxUsernameLength)
            {
                throw new ArgumentException(string.Format(Constants.ErrorMessages.UsernameNotValid, username));
            }

            var password = inputArgs[1];

            var regex = new Regex(@"([A-Z]+(.+)[\d]+)|([\d]+(.+)[A-Z]+)");
            var match = regex.Match(password);

            if (!match.Success || password.Length < Constants.MinPasswordLength || password.Length > Constants.MaxPasswordLength)
            {
                throw new ArgumentException(string.Format(Constants.ErrorMessages.PasswordNotValid, password));
            }

            var repeatPassword = inputArgs[2];

            if (!repeatPassword.Equals(password))
            {
                throw new ArgumentException(string.Format(Constants.ErrorMessages.PasswordDoesNotMatch));
            }

            var firstName = inputArgs[3];
            var lastName = inputArgs[4];

            int age;
            var isNumber = int.TryParse(inputArgs[5], out age);

            if (!isNumber || age < 0)
            {
                throw new ArgumentException(Constants.ErrorMessages.AgeNotValid);
            }

            Gender gender;
            var isValidGender = Enum.TryParse(inputArgs[6], out gender);

            if (!isValidGender)
            {
                throw new ArgumentException(Constants.ErrorMessages.GenderNotValid);
            }

            if (CommandHelper.IsUserExisting(username))
            {
                throw new InvalidOperationException(string.Format(Constants.ErrorMessages.UsernameIsTaken, username));
            }

            this.RegisterUser(username, password, firstName, lastName, age, gender);

            return $"User {username} was registered successfully.";
        }

        private void RegisterUser(string username, string password, string firstName, string lastName, int age, Gender gender)
        {
            using (var context = new TeamBuilderContext())
            {
                var user = new User
                {
                    Username = username,
                    Password = password,
                    FirstName = firstName, 
                    LastName = lastName, 
                    Age = age,
                    Gender = gender
                };

                context.Users.Add(user);
                context.SaveChanges();
            }
        }
    }
}
