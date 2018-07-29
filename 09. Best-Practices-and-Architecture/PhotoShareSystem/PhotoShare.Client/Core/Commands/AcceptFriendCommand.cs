namespace PhotoShare.Client.Core.Commands
{
    using System;
    using Contracts;
    using PhotoShare.Services.Contracts;
    using PhotoShare.Client.Core.Dtos;
    using System.Linq;

    public class AcceptFriendCommand : ICommand
    {
        private const string SUCCESSFULLY_ACCEPTED = "Friend {0} accepted {1} as a friend.";
        private const string USER_NOT_FOUND = "{0} not found.";
        private const string ALREADY_FRIENDS = "{0} is already a friend to {1}.";
        private const string NO_SUCH_REQUEST = "{0} has not added {1} as a friend.";

        private readonly IUserService userService;

        public AcceptFriendCommand(IUserService userService)
        {
            this.userService = userService;
        }

        // AcceptFriend <username1> <username2>
        public string Execute(string[] data)
        {
            var username = data[0];
            var friendUsername = data[1];

            this.ValidateUsernames(username, friendUsername);

            var userFriendsDto = this.userService.ByUsername<UserFriendsDto>(username);
            var friendFriendsDto = this.userService.ByUsername<UserFriendsDto>(friendUsername);

            this.CheckIfAlreadyFriends(userFriendsDto, friendFriendsDto);

            this.CheckIfThereIfSuchFriendRequest(userFriendsDto, friendFriendsDto);

            this.userService.AcceptFriend(userFriendsDto.Id, friendFriendsDto.Id);

            return string.Format(SUCCESSFULLY_ACCEPTED, username, friendFriendsDto.Username);
        }

        private void CheckIfThereIfSuchFriendRequest(UserFriendsDto user, UserFriendsDto friend)
        {
            var friendHasSentRequest = friend.Friends.Any(f => f.Username.Equals(user.Username)); 

            if (!friendHasSentRequest)
            {
                throw new InvalidOperationException(string.Format(NO_SUCH_REQUEST, friend.Username, user.Username));
            }
        }

        private void CheckIfAlreadyFriends(UserFriendsDto user, UserFriendsDto friend)
        {
            var userHasSentRequest = user.Friends.Any(f => f.Id.Equals(friend.Id)); 
            var friendHasSentRequest = friend.Friends.Any(f => f.Id.Equals(user.Id)); 

            if (userHasSentRequest && friendHasSentRequest)
            {
                throw new InvalidOperationException(string.Format(ALREADY_FRIENDS, friend.Username, user.Username));
            }
        }

        private void ValidateUsernames(string usernameReceivedRequest, string usernameSentRequest)
        {
            if (!this.userService.Exists(usernameReceivedRequest))
            {
                throw new ArgumentException(string.Format(USER_NOT_FOUND, usernameReceivedRequest));
            }

            if (!this.userService.Exists(usernameSentRequest))
            {
                throw new ArgumentException(string.Format(USER_NOT_FOUND, usernameSentRequest));
            }
        }
    }
}
