using BusinessLogic.BusinessLogic;
using DataAccess;

namespace BusinessLogic
{
    public class BL:IBL
    {
        public IUserService UserService { get; }
        public IMediaService MediaService { get; }
        public IRatingService RatingService { get; }
        public IRecommendationService RecommendationService { get; }
        public BL(IUserService userService, IMediaService mediaService, IRatingService ratingService, IRecommendationService recommendationService)
        {
            UserService = userService;
            MediaService = mediaService;
            RatingService = ratingService;
            RecommendationService = recommendationService; 
        }
    }
}
