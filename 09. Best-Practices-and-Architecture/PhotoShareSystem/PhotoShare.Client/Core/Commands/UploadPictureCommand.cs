namespace PhotoShare.Client.Core.Commands
{
    using System;

    using Dtos;
    using Contracts;
    using Services.Contracts;

    public class UploadPictureCommand : ICommand
    {
        private const string SUCCESSFULLY_ADDED = "Picture {0} added to album {1}.";
        private const string ALBUM_NOT_FOUND = "Album {0} not found.";
        private const string INVALID_CREDENTIALS = "Invalid credentials.";

        private readonly IUserSessionService userSessionService;
        private readonly IUserService userService;
        private readonly IPictureService pictureService;
        private readonly IAlbumService albumService;

        public UploadPictureCommand(IUserSessionService userSessionService, IUserService userService, IPictureService pictureService, IAlbumService albumService)
        {
            this.userSessionService = userSessionService;
            this.userService = userService;
            this.pictureService = pictureService;
            this.albumService = albumService;
        }

        // UploadPicture <albumName> <pictureTitle> <pictureFilePath>
        // Creates picture and atteches it to specified album.
        public string Execute(string[] data)
        {
            string albumTitle = data[0];
            string pictureTitle = data[1];
            string path = data[2];

            this.CheckIfLoggedIn();
            this.ValidateAlbum(albumTitle);

            var albumId = this.albumService.ByName<AlbumDto>(albumTitle).Id;

            this.pictureService.Create(albumId, pictureTitle, path);

            return string.Format(SUCCESSFULLY_ADDED, pictureTitle, albumTitle);
        }

        private void CheckIfLoggedIn()
        {
            var isLoggedIn = this.userSessionService.IsLoggedIn();

            if (!isLoggedIn)
            {
                throw new InvalidOperationException(INVALID_CREDENTIALS);
            }
        }

        private void ValidateAlbum(string albumTitle)
        {
            var albumExists = this.albumService.Exists(albumTitle);

            if (!albumExists)
            {
                throw new ArgumentException(string.Format(ALBUM_NOT_FOUND, albumTitle));
            }
        }
    }
}
