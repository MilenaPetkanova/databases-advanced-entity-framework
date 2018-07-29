namespace PhotoShare.Services
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using AutoMapper.QueryableExtensions;

    using Data;
    using Models;
    using Contracts;

    public class UserService : IUserService
	{
        private readonly PhotoShareContext context;

        public UserService(PhotoShareContext context)
        {
            this.context = context;
        }

        public User Register(string username, string password, string email)
        {
            var user = new User
            {
                Username = username,
                Password = password,
                Email = email,
                IsDeleted = false
            };

            this.context.Users.Add(user);

            this.context.SaveChanges();

            return user;
        }

        public void Delete(string username)
        {
            //var user = this.ByUsername<User>(username);

            var user = this.context.Users.SingleOrDefault(u => u.Username.Equals(username));

            user.IsDeleted = true;

            //this.context.Users.Remove(user);

            this.context.SaveChanges();
        }

        public void SetBornTown(int userId, int townId)
        {
            //var bornTown = this.ById<Town>(townId);

            var user = this.context.Users.Find(userId);
            var bornTown = this.context.Towns.Find(townId);

            user.BornTownId = bornTown.Id;

            this.context.SaveChanges();
        }

        public void SetCurrentTown(int userId, int townId)
        {
            //var user = this.ById<User>(userId);
            //var currentTown = this.ById<Town>(townId);

            var user = this.context.Users.Find(userId);
            var currentTown = this.context.Towns.Find(townId);

            user.CurrentTown = currentTown;

            this.context.SaveChanges();
        }

        public void ChangePassword(int userId, string password)
        {
            //var user = this.ById<User>(userId);

            var user = this.context.Users.Find(userId);

            user.Password = password;

            this.context.SaveChanges();
        }

        public Friendship AcceptFriend(int userId, int friendId)
        {
            var friendship = new Friendship
            {
                UserId = userId,
                FriendId = friendId
            };

            this.context.Friendships.Add(friendship);
            
            this.context.SaveChanges();

            return friendship;
        }

        public Friendship AddFriend(int userId, int friendId)
        {
            var friendship = new Friendship
            {
                UserId = userId,
                FriendId = friendId
            };

            this.context.Friendships.Add(friendship);

            this.context.SaveChanges();

            return friendship;
        }

        private IEnumerable<TModel> By<TModel>(Func<User, bool> predicate)
            => this.context.Users
                    .Where(predicate)
                    .AsQueryable()
                    .ProjectTo<TModel>();

        public TModel ById<TModel>(int id)
            => this.By<TModel>(i => i.Id == id).SingleOrDefault();

        public TModel ByUsername<TModel>(string username)
            => this.By<TModel>(i => i.Username == username).SingleOrDefault();

        public bool Exists(int id)
            => this.ById<User>(id) != null;

        public bool Exists(string name)
            => this.ByUsername<User>(name) != null;

    }
}