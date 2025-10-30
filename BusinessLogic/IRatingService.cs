using BusinessLogic.Models;

namespace BusinessLogic
{
    public interface IRatingService
    {
        public Task<List<Rating>> GetReviewsFromUser(User user);
        public Task<List<Favourite>> GetFavouritesFromUser(User user);
        public Task<int?> PostRating(PostRating addRating);
        public Task<bool> DeleteRatingById(int id);
        public Task<bool> Favourite(User user, Media media);
        public Task<bool> Unfavourite(User user, Media media);

    }
}