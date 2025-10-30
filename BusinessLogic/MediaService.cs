using BusinessLogic.Models;
using DataAccess;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class MediaService:IMediaService
    {
        private IDAL _dal;
        public MediaService(IDAL dal) {
            _dal = dal;
        }


        public async Task<Media?> FindMediaById(int id)
        {
            var found = await _dal.MediaRepo.FindMediaById(id);
            if (found == null)
            {
                return null;
            }
            return new Media(found.Id, found.Title, found.Description, found.ReleaseDate, found.Fsk, found.Genres, found.MediaType, found.AverageRating);
        }

        public async Task<bool> DeleteMediaById(int id)
        {
            return await _dal.MediaRepo.DeleteMedia(id);
        }
        public async Task<List<Media>> GetMedia(MediaFilter filter)
        {
            var re=new List<Media>();
            var foundMedias = await _dal.MediaRepo.GetMedia(
                new MediaFilterDAL(filter.Title, filter.MediaType, filter.ReleaseYear, filter.Genre, filter.Fsk, filter.MinRating, filter.SortBy));
            if (foundMedias == null)
            {
                return re;
            }
            foreach (var found in foundMedias)
            {
                re.Add(new Media(found.Id, found.Title, found.Description, found.ReleaseDate, found.Fsk, found.Genres, found.MediaType, found.AverageRating));
            }
            return re;
        }

        public async Task<int?> PostMedia(PostMedia postedMedia)
        {
            var addMedia = new AddMedia(postedMedia.Title, postedMedia.Description, postedMedia.ReleaseDate, postedMedia.Fsk, postedMedia.Genres, postedMedia.MediaType);
            return await _dal.MediaRepo.AddMedia(addMedia);
        }
        public async Task<bool> PutMedia(PutMedia putMedia)
        {
            var updateMedia = new UpdateMedia(putMedia.Id, putMedia.Title, putMedia.Description, putMedia.ReleaseDate, putMedia.Fsk, putMedia.Genres, putMedia.MediaType);
            return await _dal.MediaRepo.UpdateMedia(updateMedia);
        }
    }
}
