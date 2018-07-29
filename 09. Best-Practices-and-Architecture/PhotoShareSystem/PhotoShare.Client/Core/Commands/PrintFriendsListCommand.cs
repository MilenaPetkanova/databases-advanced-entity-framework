namespace PhotoShare.Client.Core.Commands
{
    using System;
    using System.Text;
    using System.Linq;

    using PhotoShare.Client.Core.Contracts;
    using PhotoShare.Client.Core.Dtos;
    using PhotoShare.Services.Contracts;

    public class PrintFriendsListCommand : ICommand
    {
        private const string NO_FRIENDS = "No friends for this user.";
        private const string INVALID_USER = "User {0} not found.";

        private readonly IUserService userService;

        public PrintFriendsListCommand(IUserService userService)
        {
            this.userService = userService;
        }

        // List usernames of all friends for given user sorted alphabetically.
        public string Execute(string[] args)
        {
            var username = args[0];

            this.ValidateUser(username);

            var userFriendsDto = this.userService.ByUsername<UserFriendsDto>(username);

            this.CheckIfUserHaveFriends(userFriendsDto);

            var sortedFriends = userFriendsDto.Friends.ToList()
                .OrderBy(u => u.Username);

            var sb = new StringBuilder();

            sb.AppendLine("Friends:");
            foreach (var friend in sortedFriends)
            {
                sb.AppendLine($"-{friend.Username}");
            }

            return sb.ToString().Trim();
        }

        private void ValidateUser(string username)
        {
            if (!this.userService.Exists(username))
            {
                throw new ArgumentException(string.Format(INVALID_USER, username));
            }
        }

        private void CheckIfUserHaveFriends(UserFriendsDto userFriendsDto)
        {
            if (!userFriendsDto.Friends.Any())
            {
                throw new ArgumentException(NO_FRIENDS);
            }
        }
    }
}
