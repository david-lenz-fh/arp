using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public interface IRatingRepository
    {
        public Task<RatingEntity?> FindRatingById(int id);
        public Task<List<RatingEntity>> GetRatingsForUser(string username);
        public Task<int?> AddRating(AddRating added);
        public Task<bool> UpdateRating(UpdateRating updated);
        public Task<bool> DeleteRatingById(int id);
        public Task<bool> Favourite(string username, int mediaId);
        public Task<bool> Unfavourite(string username, int mediaId);
        public Task<List<FavouriteEntity>> GetFavourites(string username);


    }
}
