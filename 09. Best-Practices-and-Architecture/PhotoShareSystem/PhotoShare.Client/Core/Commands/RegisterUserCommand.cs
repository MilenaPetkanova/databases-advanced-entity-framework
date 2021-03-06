﻿namespace PhotoShare.Client.Core.Commands
{
    using System;

    using Contracts;
    using PhotoShare.Services.Contracts;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;
    using PhotoShare.Client.Core.Dtos;

    public class RegisterUserCommand : ICommand
    {
        private const string SUCCESSFULLY_REGISTRERED = "User {0} was registered successfully.";
        private const string INVALID_DATA = "Invalid data.";
        private const int ARGS_COUNT = 4;
        private const string INVALID_COMMAND = "Command RegisterUser not valid.";
        private const string UNMATCHED_PASSWORDS = "Passwords do not match.";
        private const string TAKEN_USERNAME = "Username {0} is already taken.";
        private const string INVALID_CREDENTIALS = "Invalid credentials.";

        private readonly IUserSessionService userSessionService;
        private readonly IUserService userService;

        public RegisterUserCommand(IUserSessionService userSessionService, IUserService userService)
        {
           this.userSessionService = userSessionService;
           this.userService = userService;
        }

        // RegisterUser <username> <password> <repeat-password> <email>
        // Registers a new user.
        public string Execute(string[] data)
        {
            this.ValidateArgumentsCount(data);

            var username = data[0];
            var password = data[1];
            var repeatPassword = data[2];
            var email = data[3];

            var registerUserDto = new RegisterUserDto
            {
                Username = username,
                Password = password,
                Email = email
            };

            if (!this.IsValid(registerUserDto))
            {
                throw new ArgumentException(INVALID_DATA);
            }

            this.CheckIfLoggedOut();
            this.ValidateUsername(username);
            this.ValidatePasswordMatch(password, repeatPassword);

            this.userService.Register(username, password, email);

            return string.Format(SUCCESSFULLY_REGISTRERED, username);
        }

        private void ValidateArgumentsCount(string[] data)
        {
            if (!data.Length.Equals(ARGS_COUNT))
            {
                throw new ArgumentException(INVALID_COMMAND);
            }
        }

        private void CheckIfLoggedOut()
        {
            var loggedIn = this.userSessionService.IsLoggedIn();

            if (loggedIn)
            {
                throw new InvalidOperationException(INVALID_CREDENTIALS);
            }
        }

        private void ValidateUsername(string username)
        {
            if (this.userService.Exists(username))
            {
                throw new InvalidOperationException(string.Format(TAKEN_USERNAME, username));
            }
        }

        private void ValidatePasswordMatch(string password, string repeatPassword)
        {
            if (!password.Equals(repeatPassword))
            {
                throw new ArgumentException(UNMATCHED_PASSWORDS);
            }
        }

        private bool IsValid(object obj)
        {
            var validationContext = new ValidationContext(obj);
            var validationResults = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, validationContext, validationResults, true);
        }
    }
}
