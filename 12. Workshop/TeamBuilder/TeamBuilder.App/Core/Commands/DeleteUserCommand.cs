namespace TeamBuilder.App.Core.Commands
{
    using System.Linq;
    using TeamBuilder.App.Core.Commands.Contracts;
    using TeamBuilder.App.Utilities;
    using TeamBuilder.Data;

    public class DeleteUserCommand : ICommand
    {
        public string Execute(string[] inputArgs)
        {
            Check.CheckLength(0, inputArgs);
            AuthenticationManager.Authorize();

            var currentUser = AuthenticationManager.GetCurrentUser();

            using (var context = new TeamBuilderContext())
            {
                var user = context.Users.Single(u => u.Username.Equals(currentUser.Username));

                user.IsDeleted = true;
                context.SaveChanges();

                AuthenticationManager.Logout();
            }

            return $"User {currentUser.Username} was deleted successfully!";
        }
    }
}
