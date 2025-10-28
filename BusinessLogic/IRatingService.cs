using BusinessLogic.Models;

namespace BusinessLogic
{
    public interface IRatingService
    {
        public Task<List<Review>> GetReviewsFromUser(User user);
        public Task<List<Favourite>> GetFavouritesFromUser(User user);
    }
}