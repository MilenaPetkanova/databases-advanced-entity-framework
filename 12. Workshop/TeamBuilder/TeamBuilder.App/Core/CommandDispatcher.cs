namespace TeamBuilder.App.Core
{
    using System;
    using System.Linq;
    using TeamBuilder.App.Core.Commands;

    public class CommandDispatcher
    {
        public string Dispatch(string input)
        {
            var result = String.Empty;

            var inputArgs = input.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            var commandName = input.Length > 0 ? inputArgs[0] : string.Empty;
            var commandArgs = inputArgs.Skip(1).ToArray();

            switch (commandName)
            {
                case "RegisterUser":
                    var registerUser = new RegisterUserCommand();
                    result = registerUser.Execute(commandArgs);
                    break;
                case "Login":
                    var login = new LoginCommand();
                    result = login.Execute(commandArgs);
                    break;
                case "Logout":
                    var logout = new LogoutCommand();
                    result = logout.Execute(commandArgs);
                    break;
                case "DeleteUser":
                    var deleteUser = new DeleteUserCommand();
                    result = deleteUser.Execute(commandArgs);
                    break;
                case "Exit":
                    var exit = new ExitCommand();
                    exit.Execute(commandArgs);
                    break;
                
                default:
                    throw new NotSupportedException($"Command {commandName} not supported!");
            }

            return result;
        }
    }
}
