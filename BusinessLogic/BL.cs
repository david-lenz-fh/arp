using DataAccess;

namespace BusinessLogic
{
    public class BL:IBL
    {
        public IUserService UserService { get; }
        public IMediaService MediaService { get; }
        public IRatingService RatingService { get; }
        public BL(IUserService userService, IMediaService mediaService, IRatingService ratingService)
        {
            UserService = userService;
            MediaService = mediaService;
            RatingService = ratingService;
        }
    }
}
