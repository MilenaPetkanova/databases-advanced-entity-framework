namespace PhotoShare.Client.Core.Commands
{
    using System;

    using Contracts;
    using PhotoShare.Services.Contracts;
    using PhotoShare.Client.Core.Dtos;
    using System.Linq;

    public class AddFriendCommand : ICommand
    {
        private const string SUCCESSFULLY_ADDED = "Friend {0} added to {1}.";
        private const string USER_NOT_FOUND = "User {0} not found.";
        private const string ALREADY_FRIENDS = "{0} is already a friend to {1}.";
        private const string ALREADY_SENT_REQUEST = "Request is already sent.";
        private const string INVALID_CREDENTIALS = "Invalid credentials.";
        
        private readonly IUserSessionService userSessionService;
        private readonly IUserService userService;

        public AddFriendCommand(IUserSessionService userSessionService, IUserService userService)
        {
            this.userSessionService = userSessionService;
            this.userService = userService;
        }

        // AddFriend <username1> <username2>
        // The first user adds the second one as a friend. 
        public string Execute(string[] data)
        {
            var username = data[0];
            var friendUsername = data[1];

            this.ValidateUsernames(username, friendUsername);
            this.CheckIfLoggedIn(username);

            var userFrindsDto = this.userService.ByUsername<UserFriendsDto>(username);
            var friendFriendsDto = this.userService.ByUsername<UserFriendsDto>(friendUsername);

            this.CheckIfAlreadyFriends(userFrindsDto, friendFriendsDto);

            this.userService.AddFriend(userFrindsDto.Id, friendFriendsDto.Id);

            return string.Format(SUCCESSFULLY_ADDED, username, friendUsername);
        }

        private void ValidateUsernames(string username, string friendUsername)
        {
            if (!this.userService.Exists(username))
            {
                throw new ArgumentException(string.Format(USER_NOT_FOUND, username));
            }

            if (!this.userService.Exists(friendUsername))
            {
                throw new ArgumentException(string.Format(USER_NOT_FOUND, friendUsername));
            }
        }

        private void CheckIfLoggedIn(string username)
        {
            var isLoggedIn = this.userSessionService.IsLoggedIn();
            var isThisUserLoggedIn = this.userSessionService.GetUsername().Equals(username);

            if (!isLoggedIn || !isThisUserLoggedIn)
            {
                throw new InvalidOperationException(INVALID_CREDENTIALS);
            }
        }

        private void CheckIfAlreadyFriends(UserFriendsDto userFrindsDto, UserFriendsDto friendFriendsDto)
        {
            var userHasSentRequest = userFrindsDto.Friends.Any(f => f.Id.Equals(friendFriendsDto.Id)); 
            var friendHasSentRequest = friendFriendsDto.Friends.Any(f => f.Id.Equals(userFrindsDto.Id)); 

            if (userHasSentRequest && friendHasSentRequest)
            {
                throw new InvalidOperationException(string.Format(ALREADY_FRIENDS, friendHasSentRequest, userHasSentRequest));
            }
            if (userHasSentRequest && !friendHasSentRequest || !userHasSentRequest && friendHasSentRequest)
            {
                throw new InvalidOperationException(ALREADY_SENT_REQUEST);
            }
        }
    }
}
