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
        public Task<bool> DeleteRating(UpdateRating updated);
        public Task<bool> Favourite(int userId, int mediaId);
        public Task<bool> Unfavourite(int userId, int mediaId);
    }
}
