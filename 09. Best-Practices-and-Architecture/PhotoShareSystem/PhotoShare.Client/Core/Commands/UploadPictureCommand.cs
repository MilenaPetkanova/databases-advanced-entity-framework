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

        private readonly IPictureService pictureService;
        private readonly IAlbumService albumService;

        public UploadPictureCommand(IPictureService pictureService, IAlbumService albumService)
        {
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

            this.ValidateAlbum(albumTitle);

            var albumId = this.albumService.ByName<AlbumDto>(albumTitle).Id;

            this.pictureService.Create(albumId, pictureTitle, path);

            return string.Format(SUCCESSFULLY_ADDED, pictureTitle, albumTitle);
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
