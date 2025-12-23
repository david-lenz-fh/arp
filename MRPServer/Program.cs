using System.Net;
using System.Text;
using API;
using BusinessLogic;
using MRPServer;
using DataAccess;


class Program
{
    static void Main(string[] args)
    {
        //Data Access Layer
        PostgresDB postgresDB = PostgresDB.Initialize("Host=localhost;Port=5432;Username=postgres;Password=1234;Database=mrp_db;");
        IMediaRepository mediaRepository = new MediaRepository(postgresDB);
        IRatingRepository ratingRepository = new RatingRepository(postgresDB);
        IUserRepository userRepository = new UserRepository(postgresDB);
        IDAL dal = new DAL(mediaRepository, userRepository, ratingRepository);

        //Business Layer
        IUserService userService = new UserService(dal);
        IMediaService mediaService = new MediaService(dal,userService);
        IRatingService ratingService = new RatingService(dal, mediaService, userService);
        IRecommendationService recommendationService = new RecommendationService(dal, mediaService, userService, ratingService);
        IBL businessLayer = new BL(userService, mediaService, ratingService, recommendationService);

        //API Layer
        IRatingHandler ratingHandler = new RatingHandler(businessLayer);
        IMediaHandler mediaHandler = new MediaHandler(businessLayer);
        IUserHandler userHandler = new UserHandler(businessLayer);
        IAPI api = new API.API(userHandler, mediaHandler, ratingHandler);

        //Server
        MRPServer.MRPServer mrp = new MRPServer.MRPServer(api);
        mrp.Listen();
    }
}
