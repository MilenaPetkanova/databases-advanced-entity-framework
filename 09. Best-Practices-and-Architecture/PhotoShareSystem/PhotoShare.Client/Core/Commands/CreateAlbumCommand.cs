namespace PhotoShare.Client.Core.Commands
{
    using System;

    using Contracts;
    using Services.Contracts;
    using System.Linq;
    using PhotoShare.Client.Core.Dtos;
    using PhotoShare.Models.Enums;

    public class CreateAlbumCommand : ICommand
    {
        private const string SUCCESSFULLY_CREATED = "Album {0} successfully created.";
        private const string USER_NOT_FOUND = "User {0} not found.";
        private const string ALBUM_ALREADY_EXISTS = "Album {0} already exists.";
        private const string COLOR_NOT_FOUND = "Color {0} not found.";
        private const string INVALID_TAGS = "Invalid tags.";
        private const string INVALID_CREDENTIALS = "Invalid credentials.";

        private readonly IUserSessionService userSessionService;
        private readonly IAlbumService albumService;
        private readonly IUserService userService;
        private readonly ITagService tagService;

        public CreateAlbumCommand(IUserSessionService userSessionService, IAlbumService albumService, IUserService userService, ITagService tagService)
        {
            this.userSessionService = userSessionService;
            this.albumService = albumService;
            this.userService = userService;
            this.tagService = tagService;
        }

        // CreateAlbum <username> <albumTitle> <BgColor> <tag1> <tag2>...<tagN>
        // Adds an album. Album names should be unique.
        public string Execute(string[] data)
        {
            var username = data[0];
            var albumTitle = data[1];
            var bgColor = data[2];
            var tags = data.Skip(3).ToArray();

            this.CheckIfLoggedIn();
            this.ValidateUser(username);
            this.ValidateAlbumTitle(albumTitle);
            this.ValidateBgColor(bgColor);
            this.ValidateTags(tags);

            var userId = this.userService.ByUsername<UserDto>(username).Id;

            this.albumService.Create(userId, albumTitle, bgColor, tags);

            return string.Format(SUCCESSFULLY_CREATED, albumTitle);
        }

        private void CheckIfLoggedIn()
        {
            var isLoggedIn = this.userSessionService.IsLoggedIn();

            if (!isLoggedIn)
            {
                throw new InvalidOperationException(INVALID_CREDENTIALS);
            }
        }

        private void ValidateUser(string username)
        {
            if (!this.userService.Exists(username))
            {
                throw new ArgumentException(string.Format(USER_NOT_FOUND, username));
            }
        }

        private void ValidateAlbumTitle(string albumTitle)
        {
            if (this.albumService.Exists(albumTitle))
            {
                throw new ArgumentException(string.Format(ALBUM_ALREADY_EXISTS, albumTitle));
            }
        }

        private void ValidateBgColor(string bgColor)
        {
            Color color;

            if (!Enum.TryParse(bgColor, out color))
            {
                throw new ArgumentException(string.Format(COLOR_NOT_FOUND, bgColor));
            }
        }

        private void ValidateTags(string[] tags)
        {
            foreach (var tag in tags)
            {
                if (!this.tagService.Exists(tag))
                {
                    throw new ArgumentException(INVALID_TAGS);
                }
            }
        }
    }
}
