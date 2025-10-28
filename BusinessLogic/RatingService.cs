using BusinessLogic.Models;
using DataAccess;
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

        public async Task<List<Review>> GetReviewsFromUser(User user)
        {
            var re = new List<Review>();
            var found=await _dal.RatingRepo.GetRatingsForUser(user.Username);
            foreach (var rating in found)
            {
                var media=await _mediaService.FindMediaById(rating.MediaId);
                if (media == null) {
                    continue;
                }
                re.Add(new Review(rating.Id, user, media, rating.Comment, rating.Rating));
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

    }
}
