namespace PhotoShare.Client.Core.Commands
{
    using System;
    using PhotoShare.Client.Core.Contracts;
    using PhotoShare.Services.Contracts;

    public class AddTagCommand : ICommand
    {
        private const string SUCCESSFULLY_ADDED = "Tag {0} was added successfully.";
        private const string ALREADY_EXISTS = "Tag {0} already exists.";
        private const string INVALID_CREDENTIALS = "Invalid credentials.";

        private readonly IUserSessionService userSessionService;
        private readonly ITagService tagService;
        
        public AddTagCommand(IUserSessionService userSessionService, ITagService tagService)
        {
            this.userSessionService = userSessionService;
            this.tagService = tagService;
        }

        // AddTag <tag>
        // Adds a tag. Tag names should be unique.
        public string Execute(string[] args)
        {
            var tag = args[0];

            this.CheckIfLoggedIn();
            this.ValidateTag(tag);

            this.tagService.AddTag(tag);

            return string.Format(SUCCESSFULLY_ADDED, tag);
        }

        private void CheckIfLoggedIn()
        {
            var isLoggedIn = this.userSessionService.IsLoggedIn();

            if (!isLoggedIn)
            {
                throw new InvalidOperationException(INVALID_CREDENTIALS);
            }
        }

        private void ValidateTag(string tag)
        {
            if (this.tagService.Exists(tag))
            {
                throw new ArgumentException(string.Format(ALREADY_EXISTS, tag));
            }
        }
    }
}
