using BusinessLogic;
using BusinessLogic.Models;
using DataAccess;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace Unit_Test
{
    public class MockDAL():IDAL
    {
        public IMediaRepository MediaRepo => new MediaRepositoryMocked();

        public IUserRepository UserRepo => new UserRepositoryMocked();

        public IRatingRepository RatingRepo => new RatingRepositoryMocked();
    }
    class UserRepositoryMocked : IUserRepository
    {
        public Task<bool> AddActivityPoints(string username, int points)
        {
            if (username == "TestUser")
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<bool> AddUser(UserEntity user)
        {
            return Task.FromResult(true);
        }

        public Task<bool> DeleteUser(string username)
        {
            if (username == "TestUser")
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<UserEntity?> FindUserByName(string username)
        {
            if (username == "TestUser")
            {
                return Task.FromResult<UserEntity?>(new UserEntity("TestUser", "a", "testmail@g.c", "drama"));
            }
            return Task.FromResult<UserEntity?>(null);
        }

        public Task<List<UserActivity>> GetLeaderboard(int topXusers)
        {
            var re = new List<UserActivity> { new UserActivity("TestUser", 5), new UserActivity("TestUser2", 4), new UserActivity("TestUser3", 1) };
            return Task.FromResult(re.GetRange(0, Math.Abs(topXusers)));
        }

        public Task<bool> UpdateUser(UserEntity updated)
        {
            if (updated.Username == "TestUser")
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
    class MediaRepositoryMocked() : IMediaRepository
    {
        public Task<int?> AddMedia(AddMedia media)
        {
            return Task.FromResult<int?>(2);
        }

        public Task<bool> DeleteMedia(int id)
        {
            if (id==1)
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<MediaEntity?> FindMediaById(int id)
        {
            if (id == 1)
            {
                return Task.FromResult<MediaEntity?>(new MediaEntity(1, "TestMedia","media for a test", null, 10, new List<string>(), null, 4.2m, "TestUser"));
            }
            return Task.FromResult<MediaEntity?>(null);
        }

        public Task<List<string>> GetGenres()
        {
            throw new NotImplementedException();
        }

        public Task<List<MediaEntity>> GetMedia(MediaFilterDAL filter)
        {
            return Task.FromResult(new List<MediaEntity> { new MediaEntity(1, "TestMedia", "media for a test", null, 10, new List<string>(), null, 4.2m, "TestUser") });
        }

        public Task<bool> UpdateMedia(UpdateMedia media)
        {
            if(media.Id == 1){
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
    class RatingRepositoryMocked() : IRatingRepository
    {
        public Task<int?> AddRating(AddRating added)
        {
            if(added.MediaId==1 && added.Username == "TestUser") {
                return Task.FromResult<int?>(0);
            }
            return Task.FromResult<int?>(null);
        }

        public Task<bool> DeleteRatingById(int id)
        {
            if (id == 0)
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<bool> Favourite(string username, int mediaId)
        {

            if (mediaId == 1 && username == "TestUser")
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<RatingEntity?> FindRatingById(int id)
        {
            if(id == 0)
            {
                return Task.FromResult<RatingEntity?>(new RatingEntity(0, "TestUser", 1, "WOOW", 4, false, null));
            }
            return Task.FromResult<RatingEntity?>(null);
        }

        public Task<List<FavouriteEntity>> GetFavourites(string username)
        {
            if (username == "TestUser")
            {
                return Task.FromResult(new List<FavouriteEntity>{new FavouriteEntity("TestUser", 1)});
            }
            return Task.FromResult(new List<FavouriteEntity>());
        }

        public Task<List<RatingEntity>> GetRatingsForUser(string username)
        {
            if(username == "TestUser")
            {
                return Task.FromResult(new List<RatingEntity> { new RatingEntity(0, "TestUser", 1, "WOOW", 4, false, null) });
            }
            return Task.FromResult(new List<RatingEntity>());
        }

        public Task<bool> LikeRating(string username, int ratingId)
        {
            if (username == "TestUser" && ratingId == 0)
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<bool> Unfavourite(string username, int mediaId)
        {
            if (mediaId == 1 && username == "TestUser")
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<bool> UpdateRating(UpdateRating updated)
        {

            if (updated.Id == 0)
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
    public class MockBL() : IBL
    {
        public IUserService UserService => new UserServiceMocked();

        public IMediaService MediaService => new MediaServiceMocked();

        public IRatingService RatingService => throw new NotImplementedException();

        public IRecommendationService RecommendationService => throw new NotImplementedException();
    }
    class UserServiceMocked : IUserService
    {
        public Task<Result<User>> AuthenticateUserByToken(string authenticationToken)
        {
            if(authenticationToken == "token")
            {
                return Task.FromResult(new Result<User>(new User("TestUser", "TestPasswort", "test@g.c", "drama"), new ResultResponse(BL_Response.OK, null)));
            }
            if (authenticationToken == "token2")
            {
                return Task.FromResult(new Result<User>(new User("TestUser2", "TestPasswort", "test@g.c", "drama"), new ResultResponse(BL_Response.OK, null)));
            }
            return Task.FromResult(new Result<User>(null, new ResultResponse(BL_Response.AuthenticationFailed, null)));
        }

        public Task<Result<User>> FindUserByName(string username)
        {
            if (username == "TestUser")
            {
                return Task.FromResult(new Result<User>(new User("TestUser", "TestPasswort", "test@g.c", "drama"), new ResultResponse(BL_Response.OK, null)));
            }
            return Task.FromResult(new Result<User>(null, new ResultResponse(BL_Response.NotFound, null)));
        }

        public Task<Result<List<UserRank>>> Leaderboard()
        {
            return Task.FromResult(new Result<List<UserRank>>(new List<UserRank> { new UserRank(1, 5, "TestUser"), new UserRank(2, 3, "TestUser2") }, new ResultResponse(BL_Response.OK, null)));
        }

        public Task<Result<string>> Login(Login credentials)
        {
            if (credentials.Username == "TestUser" && credentials.Password=="TestPassword")
            {
                return Task.FromResult(new Result<string>("token", new ResultResponse(BL_Response.OK, null)));
            }
            return Task.FromResult(new Result<string>(null, new ResultResponse(BL_Response.AuthenticationFailed, null)));
        }

        public Task<Result<string>> Register(Login credentials)
        {
            if (credentials.Username == "TestUser" && credentials.Password == "TestPassword")
            {
                return Task.FromResult(new Result<string>("token", new ResultResponse(BL_Response.OK, null)));
            }
            return Task.FromResult(new Result<string>(null, new ResultResponse(BL_Response.AuthenticationFailed, null)));
        }

        public Task<ResultResponse> UpdateProfile(string authenticationToken, Profile updatedProfile)
        {
            if (authenticationToken=="token")
            {
                return Task.FromResult(new ResultResponse(BL_Response.OK, null));
            }
            return Task.FromResult(new ResultResponse(BL_Response.AuthenticationFailed, null));
        }
    }
    class MediaServiceMocked() : IMediaService
    {
        public Task<ResultResponse> DeleteMediaById(string authenticationToken, int id)
        {
            if (authenticationToken != "token2" && authenticationToken!="token")
            {
                return Task.FromResult(new ResultResponse(BL_Response.AuthenticationFailed,null));
            }
            if (authenticationToken == "token" && id == 1)
            {
                return Task.FromResult(new ResultResponse(BL_Response.OK, null));
            }
            if (authenticationToken == "token2" && id == 1)
            {
                return Task.FromResult(new ResultResponse(BL_Response.Unauthorized, null));
            }
            return Task.FromResult(new ResultResponse(BL_Response.NotFound, null));
        }

        public Task<Result<Media>> FindMediaById(int id)
        {
            if(id == 1)
            {
                return Task.FromResult(new Result<Media>(
                    new Media(1, "TestMedia", "Just to test", null, null, new List<string>(), null, 4.2m, new User("TestUser", "TestPasswort", null, null)), 
                    new ResultResponse(BL_Response.OK, null)));
            }
            return Task.FromResult(new Result<Media>(null, new ResultResponse(BL_Response.NotFound, null)));
        }

        public Task<Result<List<Media>>> GetMedia(MediaFilter? filter)
        {            
            return Task.FromResult(new Result<List<Media>>(
                new List<Media>{ new Media(1, "TestMedia", "Just to test", null, null, new List<string>(), null, 4.2m, new User("TestUser", "TestPasswort", null, null))}, 
                new ResultResponse(BL_Response.OK, null)));
        }

        public Task<Result<int?>> PostMedia(string authenticationToken, PostMedia newMedia)
        {
            if(authenticationToken == "token")
            {
                return Task.FromResult(new Result<int?>(2, new ResultResponse(BL_Response.OK, null)));
            }
            return Task.FromResult(new Result<int?>(null, new ResultResponse(BL_Response.AuthenticationFailed, null)));
        }

        public Task<ResultResponse> PutMedia(string authenticationToken, PutMedia putMedia)
        {
            if (authenticationToken != "token2" && authenticationToken != "token")
            {
                return Task.FromResult(new ResultResponse(BL_Response.AuthenticationFailed, null));
            }
            if (authenticationToken == "token" && putMedia.Id == 1)
            {
                return Task.FromResult(new ResultResponse(BL_Response.OK, null));
            }
            if (authenticationToken == "token2" && putMedia.Id == 1)
            {
                return Task.FromResult(new ResultResponse(BL_Response.Unauthorized, null));
            }
            return Task.FromResult(new ResultResponse(BL_Response.NotFound, null));
        }
    }
}
