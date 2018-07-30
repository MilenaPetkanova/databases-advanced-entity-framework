namespace PhotoShare.Client.Core.Commands
{
    using System;

    using PhotoShare.Client.Core.Contracts;
    using PhotoShare.Services.Contracts;
    using PhotoShare.Client.Core.Dtos;

    public class LoginCommand : ICommand
    {
        private const string SUCCESSFULLY_LOGGED_IN = "User {0} successfully logged in.";
        private const string INVALID_USER_OR_PASSWORD = "Invalid username or password.";
        private const string ALREADY_LOGGED_IN = "You should logout first.";
        private const string INVALID_CREDENTIALS = "Invalid credentials.";

        private readonly IUserService userService;
        private readonly IUserSessionService userSessionService;

        public LoginCommand(IUserService userService, IUserSessionService userSessionService)
        {
            this.userService = userService;
            this.userSessionService = userSessionService;
        }

        // Login <username> <password>
        // Logs a user into the system and keep a reference to it until the “Logout” command is called.
        public string Execute(string[] args)
        {
            var username = args[0];
            var password = args[1];

            this.ValidateUsernameAndPassword(username, password);
            this.CheckIfAlreadyLoggedIn();

            this.userSessionService.LogIn(username);

            return string.Format(SUCCESSFULLY_LOGGED_IN, username);
        }

        private void ValidateUsernameAndPassword(string username, string password)
        {
            var usernameExists = this.userService.Exists(username);

            if (!usernameExists)
            {
                throw new ArgumentException(INVALID_USER_OR_PASSWORD);
            }

            var userDto = this.userService.ByUsername<UserDto>(username);
            var passwordMatches = userDto.Password.Equals(password);

            if (!passwordMatches)
            {
                throw new ArgumentException(INVALID_USER_OR_PASSWORD);
            }
        }

        private void CheckIfAlreadyLoggedIn()
        {
            var loggedIn = this.userSessionService.IsLoggedIn();

            if (loggedIn)
            {
                throw new ArgumentException(ALREADY_LOGGED_IN);
            }
        }
    }
}
