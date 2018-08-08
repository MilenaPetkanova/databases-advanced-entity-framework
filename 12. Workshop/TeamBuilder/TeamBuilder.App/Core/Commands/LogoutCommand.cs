namespace TeamBuilder.App.Core.Commands
{
    using TeamBuilder.App.Core.Commands.Contracts;
    using TeamBuilder.App.Utilities;

    public class LogoutCommand : ICommand
    {
        public string Execute(string[] inputArgs)
        {
            Check.CheckLength(0, inputArgs);
            AuthenticationManager.Authorize();
            var currentUser = AuthenticationManager.GetCurrentUser();

            AuthenticationManager.Logout();

            return $"User {currentUser.Username} successfully logged out!";
        }
    }
}
