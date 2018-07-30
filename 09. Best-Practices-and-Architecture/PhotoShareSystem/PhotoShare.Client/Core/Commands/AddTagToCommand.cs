namespace PhotoShare.Client.Core.Commands
{
    using System;

    using Contracts;
    using PhotoShare.Services.Contracts;
    using PhotoShare.Client.Core.Dtos;

    public class AddTagToCommand : ICommand
    {
        private const string SUCCESSFULLY_ADDED = "Tag {0} added to {1}.";
        private const string EITHER_DO_NOT_EXIST = "Either tag or album do not exist.";
        private const string INVALID_CREDENTIALS = "Invalid credentials.";

        private readonly IUserSessionService userSessionService;
        private readonly IAlbumTagService albumTagService;
        private readonly IAlbumService albumService;
        private readonly ITagService tagService;

        public AddTagToCommand(IUserSessionService userSessionService, IAlbumTagService albumTagService, IAlbumService albumService, ITagService tagService)
        {
            this.userSessionService = userSessionService;
            this.albumTagService = albumTagService;
            this.albumService = albumService;
            this.tagService = tagService;
        }

        // AddTagTo <albumName> <tag>
        // Adds a tag.
        public string Execute(string[] args)
        {
            var albumName = args[0];
            var tag = args[1];

            this.CheckIfLoggedIn();
            this.ValidateTagAndAlbum(albumName, tag);

            var albumId = this.albumService.ByName<AlbumDto>(albumName).Id;
            var tagId = this.tagService.ByName<TagDto>(tag).Id;

            this.albumTagService.AddTagTo(albumId, tagId);

            return string.Format(SUCCESSFULLY_ADDED, tag, albumName);
        }

        private void CheckIfLoggedIn()
        {
            var isLoggedIn = this.userSessionService.IsLoggedIn();

            if (!isLoggedIn)
            {
                throw new InvalidOperationException(INVALID_CREDENTIALS);
            }
        }

        private void ValidateTagAndAlbum(string albumName, string tag)
        {
            if (!this.albumService.Exists(albumName) || !this.tagService.Exists(tag))
            {
                throw new ArgumentException(EITHER_DO_NOT_EXIST);
            }
        }
    }
}
