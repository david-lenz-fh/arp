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
    public class RatingService:IRatingService
    {
        private IDAL _dal;
        private IUserService _userService;
        private IMediaService _mediaService;
        public RatingService(IDAL dal, IMediaService mediaService, IUserService userService)
        {
            _dal = dal;
            _userService = userService;
            _mediaService = mediaService;
        }

        public async Task<List<Rating>> GetReviewsFromUser(User user)
        {
            var re = new List<Rating>();
            var found=await _dal.RatingRepo.GetRatingsForUser(user.Username);
            foreach (var rating in found)
            {
                var media=await _mediaService.FindMediaById(rating.MediaId);
                if (media == null) {
                    continue;
                }
                re.Add(new Rating(rating.Id, user, media, rating.Comment, rating.Rating));
            }
            return re;
        }
        public async Task<List<Favourite>> GetFavouritesFromUser(User user)
        {
            var re = new List<Favourite>();
            var found = await _dal.RatingRepo.GetFavourites(user.Username);
            foreach (var favourite in found)
            {
                var media = await _mediaService.FindMediaById(favourite.MediaId);
                if (media == null)
                {
                    continue;
                }
                re.Add(new Favourite(user, media));
            }
            return re;
        }
        public async Task<int?> PostRating(PostRating addRating)
        {
            return await _dal.RatingRepo.AddRating(new AddRating(addRating.Media.Id, addRating.User.Username, addRating.Comment, addRating.Stars));
        }

        public async Task<bool> Favourite(User user, Media media)
        {
            return await _dal.RatingRepo.Favourite(user.Username, media.Id);
        }

        public async Task<bool> Unfavourite(User user, Media media)
        {
            return await _dal.RatingRepo.Unfavourite(user.Username, media.Id);
        }
    }
}
