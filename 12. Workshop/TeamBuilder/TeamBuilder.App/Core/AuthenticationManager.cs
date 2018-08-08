namespace TeamBuilder.App.Core
{
    using System;
    using TeamBuilder.App.Utilities;
    using TeamBuilder.Models;

    public class AuthenticationManager
    {
        private static User currentUser;

        public static void Login(User user)
        {
            currentUser = user;
        }

        public static void Logout()
        {
            currentUser = null;
        }

        public static void Authorize()
        {
            if (currentUser == null)
            {
                throw new ArgumentException(Constants.ErrorMessages.LoginFirst);
            }
        }

        public static bool IsAuthenticated()
        {
            if (currentUser != null)
            {
                throw new ArgumentException(Constants.ErrorMessages.LogoutFirst);
            }

            return true;
        }

        public static User GetCurrentUser()
        {
            return currentUser;
        }
    }
}
