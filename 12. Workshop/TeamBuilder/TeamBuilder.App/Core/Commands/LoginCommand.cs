namespace TeamBuilder.App.Core.Commands
{
    using System;
    using System.Linq;
    using TeamBuilder.App.Core.Commands.Contracts;
    using TeamBuilder.App.Utilities;
    using TeamBuilder.Data;
    using TeamBuilder.Models;

    public class LoginCommand : ICommand
    {
        public string Execute(string[] inputArgs)
        {
            Check.CheckLength(2, inputArgs);

            var username = inputArgs[0];
            var password = inputArgs[1];

            if (!AuthenticationManager.IsAuthenticated())
            {
                throw new InvalidOperationException (Constants.ErrorMessages.LoginFirst);
            }

            var user = this.GetUserByCredentials(username, password);

            if (user == null || user.IsDeleted == true)
            {
                throw new ArgumentException(Constants.ErrorMessages.UserOrPasswordIsInvalid);
            }

            AuthenticationManager.Login(user);

            return $"User {user.Username} successfully logged in!";
        }

        private User GetUserByCredentials(string username, string password)
        {
            using (var context = new TeamBuilderContext())
            {
                var user = context.Users.SingleOrDefault(u => u.Username.Equals(username) && u.Password.Equals(password));
                return user;
            }
        }
    }
}
