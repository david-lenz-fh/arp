using BusinessLogic.Models;

namespace BusinessLogic
{
    public interface IRecommendationService
    {
        public Task<Result<List<(Media, decimal)>>> GetRecommendations(string authenticationToken);
    }
}