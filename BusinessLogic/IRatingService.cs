using BusinessLogic.Models;
using DataAccess.Entities;
using System.Net;

namespace BusinessLogic
{
    public interface IRatingService
    {
        public Task<Result<List<Rating>>> GetRatingsFromUser(string username, string? authenticationToken);
        public Task<Result<List<Favourite>>> GetFavouritesFromUser(string username);
        public Task<Result<int?>> PostRating(string authenticationToken, PostRating addRating);
        public Task<ResultResponse> ConfirmComment(string authenticationToken, int ratingId);
        public Task<ResultResponse> PutRating(string authenticationToken, PutRating updatedRating);
        public Task<ResultResponse> DeleteRatingById(string authenticationToken, int id);
        public Task<Result<Rating>> FindRatingById(int id);
        public Task<ResultResponse> Favourite(string authenticationToken, int mediaId);
        public Task<ResultResponse> Unfavourite(string authenticationToken, int mediaId);

    }
}   