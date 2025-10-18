﻿using System.Net;
using System.Text;
using API;
using BusinessLogic;
using DataAccess;
using mrp;


class Program
{
    static async Task Main(string[] args)
    {
        //Data Access Layer
        PostgresDB postgresDB = PostgresDB.Initialize();
        IMediaRepository mediaRepository = new MediaRepository(postgresDB);
        IRatingRepository ratingRepository = new RatingRepository(postgresDB);
        IUserRepository userRepository = new UserRepository(postgresDB);
        IDAL dal = new DAL(mediaRepository, userRepository, ratingRepository);

        //Business Layer
        IRatingService ratingService = new RatingService(dal);
        IUserService userService = new UserService(dal);
        IMediaService mediaService = new MediaService(dal);
        IBL businessLayer = new BL(userService, mediaService, ratingService);

        //API Layer
        IRatingHandler ratingHandler = new RatingHandler(businessLayer);
        IMediaHandler mediaHandler = new MediaHandler(businessLayer);
        IUserHandler userHandler = new UserHandler(businessLayer);
        IAPI api = new API.API(userHandler, mediaHandler, ratingHandler);

        //Server
        MRPServer mrp = new MRPServer(api);

        var s= await dal.MediaRepo.FindGenreById(1);
        mrp.Listen();
    }
}
