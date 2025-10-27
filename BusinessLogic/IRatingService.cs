using BusinessLogic.Models;

namespace BusinessLogic
{
    public interface IRatingService
    {
        public Task<List<Review>> GetReviewsFromUser(User username);
    }
}