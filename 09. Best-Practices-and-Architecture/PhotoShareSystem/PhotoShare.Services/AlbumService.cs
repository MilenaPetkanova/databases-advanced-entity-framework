namespace PhotoShare.Services
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using AutoMapper.QueryableExtensions;

    using Data;
    using Models;
    using Contracts;
    using PhotoShare.Models.Enums;

    public class AlbumService : IAlbumService
    {
        private readonly PhotoShareContext context;

        public AlbumService(PhotoShareContext context)
        {
            this.context = context;
        }

        public TModel ById<TModel>(int id)
            => By<TModel>(a => a.Id == id).SingleOrDefault();
        
        public TModel ByName<TModel>(string name)
            => By<TModel>(a => a.Name == name).SingleOrDefault();

        public bool Exists(int id)
            => ById<Album>(id) != null;

        public bool Exists(string name)
            => ByName<Album>(name) != null;

        private IEnumerable<TModel> By<TModel>(Func<Album, bool> predicate)
            => this.context.Albums.Where(predicate).AsQueryable().ProjectTo<TModel>();

        public Album Create(int userId, string albumTitle, string BgColor, string[] tags)
        {
            var bgColorAsEnum = Enum.Parse<Color>(BgColor);

            var album = new Album
            {
                Name = albumTitle,
                BackgroundColor = bgColorAsEnum
            };

            this.context.Albums.Add(album);

            foreach (var tag in tags)
            {
                var tagId = this.context.Tags
                    .FirstOrDefault(t => t.Name.Equals(tag)).Id;

                var albumTag = new AlbumTag
                {
                    Album = album,
                    TagId = tagId
                };

                this.context.AlbumTags.Add(albumTag);
            }

            this.context.SaveChanges();

            return album;
        }
    }
}
