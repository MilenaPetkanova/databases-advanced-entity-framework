namespace PhotoShare.Client.Core.Commands
{
    using System;

    using Dtos;
    using Contracts;
    using Services.Contracts;

    public class DeleteUserCommand : ICommand
    {
        private const string SUCCESSFULLY_DELETED = "User {0} was deleted successfully.";
        private const string USER_NOT_FOUND = "User {0} not found.";
        private const string ALREADY_DELETED = "User {0} is already deleted.";

        private readonly IUserService userService;

        public DeleteUserCommand(IUserService userService)
        {
            this.userService = userService;
        }

        // DeleteUser <username>
        // Deletes a user.
        public string Execute(string[] data)
        {
            string username = data[0];

            this.ValidateUser(username);

            this.userService.Delete(username);

            return string.Format(SUCCESSFULLY_DELETED, username);
        }

        private void ValidateUser(string username)
        {
            var userExists = this.userService.Exists(username);

            if (!userExists)
            {
                throw new ArgumentException(string.Format(USER_NOT_FOUND, username));
            }

            var user = this.userService.ByUsername<UserDto>(username);

            if (user.IsDeleted.Value)
            {
                throw new InvalidOperationException(string.Format(ALREADY_DELETED, username));
            }
        }
    }
}
