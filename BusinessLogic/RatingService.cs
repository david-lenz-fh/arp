using BusinessLogic.Models;
using DataAccess;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace BusinessLogic
{
    public class RatingService:IRatingService
    {
        private IDAL _dal;
        private IUserService _userService;
        private IMediaService _mediaService;
        public RatingService(IDAL dal, IMediaService mediaService, IUserService userService)
        {
            _dal = dal;
            _userService = userService;
            _mediaService = mediaService;
        }

        public async Task<Result<List<Rating>>> GetRatingsFromUser(string username, string? authenticationToken)
        {
            var returnValue = new List<Rating>();
            var user = await _userService.FindUserByName(username);
            if (user.Value == null)
            {
                return new Result<List<Rating>>(null, user.Response);
            }
            bool isAuthenticated = false;
            if (authenticationToken != null)
            {
                var authenticatedUser = await _userService.AuthenticateUserByToken(authenticationToken);
                if (authenticatedUser.Value != null && authenticatedUser.Value.Username == user.Value.Username)
                {
                    isAuthenticated = true;
                }
            }
            var found=await _dal.RatingRepo.GetRatingsForUser(username);
            foreach (var rating in found)
            {
                var media=await _mediaService.FindMediaById(rating.MediaId);
                if (media.Value == null) {
                    continue;
                }
                string? comment=rating.Confirmed||isAuthenticated ? rating.Comment : null;
                
                returnValue.Add(new Rating(rating.Id, user.Value, media.Value, comment, rating.Stars, rating.TimeStamp));
            }
            return new Result<List<Rating>>(returnValue, new ResultResponse(BL_Response.OK, null));
        }
        public async Task<Result<List<Favourite>>> GetFavouritesFromUser(string username)
        {
            var returnValue = new List<Favourite>(); 
            var user = await _userService.FindUserByName(username);
            if (user.Value == null)
            {
                return new Result<List<Favourite>>(null, user.Response);
            }
            var found = await _dal.RatingRepo.GetFavourites(username);
            foreach (var favourite in found)
            {
                var media = await _mediaService.FindMediaById(favourite.MediaId);
                if (media.Value == null)
                {
                    continue;
                }
                returnValue.Add(new Favourite(user.Value, media.Value));
            }
            return new Result<List<Favourite>>(returnValue, new ResultResponse(BL_Response.OK, null));
        }
        public async Task<Result<int?>> PostRating(string authenticationToken, PostRating addRating)
        {
            if (addRating.Stars > 5 || addRating.Stars < 1)
            {
                return new Result<int?>(null, new ResultResponse(BL_Response.BadParameters, "Stars must be between 5 and 1"));
            }
            var user = await _userService.AuthenticateUserByToken(authenticationToken);
            if (user.Value == null) {
                return new Result<int?>(null, user.Response);
            }
            
            var returnValue = await _dal.RatingRepo.AddRating(new AddRating(addRating.MediaId, user.Value.Username, addRating.Comment, addRating.Stars));
            if(returnValue == null)
            {
                return new Result<int?>(returnValue, new ResultResponse(BL_Response.InternalError, "Couldnt Post"));
            }
            await _dal.UserRepo.AddActivityPoints(user.Value.Username, 3);
            return new Result<int?>(returnValue, new ResultResponse(BL_Response.OK, "Rating was created"));
        }

        public async Task<ResultResponse> Favourite(string authenticationToken, int mediaId)
        {
            var user = await _userService.AuthenticateUserByToken(authenticationToken);
            if (user.Value == null) {
                return user.Response;
            }
            if(!await _dal.RatingRepo.Favourite(user.Value.Username, mediaId)){
                return new ResultResponse(BL_Response.InternalError, "Couldnt favourite");
            }
            await _dal.UserRepo.AddActivityPoints(user.Value.Username, 1);
            return new ResultResponse(BL_Response.OK, "favourited");
        }

        public async Task<ResultResponse> Unfavourite(string authenticationToken, int mediaId)
        {
            var user = await _userService.AuthenticateUserByToken(authenticationToken);
            if (user.Value == null)
            {
                return user.Response;
            }
            if (!await _dal.RatingRepo.Unfavourite(user.Value.Username, mediaId))
            {
                return new ResultResponse(BL_Response.InternalError, "Couldnt unfavourite");
            }
            return new ResultResponse(BL_Response.OK, "unfavourited");
        }

        public async Task<ResultResponse> DeleteRatingById(string authenticationToken, int id)
        {
            var user = await _userService.AuthenticateUserByToken(authenticationToken);
            if(user.Value == null)
            {
                return user.Response;
            }
            var rating = await FindRatingById(id);
            if(rating.Value == null)
            {
                return rating.Response;
            }
            if (user.Value.Username != rating.Value.User.Username)
            {
                return new ResultResponse(BL_Response.Unauthorized,"Not the author of this rating");
            }
            if(!await _dal.RatingRepo.DeleteRatingById(id))
            {
                return new ResultResponse(BL_Response.InternalError, "Couldnt delete rating");
            }
            return new ResultResponse(BL_Response.OK, "rating deleted");
        }

        public async Task<ResultResponse> PutRating(string authenticationToken, PutRating updatedRating)
        {
            var user = await _userService.AuthenticateUserByToken(authenticationToken);
            if (user.Value == null)
            {
                return user.Response;
            }
            var rating = await FindRatingById(updatedRating.RatingId);
            if (rating.Value == null)
            {
                return rating.Response;
            }
            if(user.Value.Username != rating.Value.User.Username)
            {
                return new ResultResponse(BL_Response.Unauthorized,"Not the author of this rating");
            }
            if (!await _dal.RatingRepo.UpdateRating(new UpdateRating(updatedRating.RatingId, updatedRating.Comment, updatedRating.Stars, null)))
            {
                return new ResultResponse(BL_Response.InternalError, "Couldnt update rating");
            }
            return new ResultResponse(BL_Response.OK, "rating updated");
        }
        
        public async Task<Result<Rating>> FindRatingById(int id)
        {
            var found=await _dal.RatingRepo.FindRatingById(id);
            if(found == null)
            {
                return new Result<Rating>(null, new ResultResponse(BL_Response.NotFound, "Rating not found"));
            }
            var user = await _userService.FindUserByName(found.Username);
            if (user.Value == null)
            {
                return new Result<Rating>(null, new ResultResponse(BL_Response.CorruptedData, "Corrupted Data"));
            }
            var media = await _mediaService.FindMediaById(found.MediaId);
            if(media.Value == null)
            {
                return new Result<Rating>(null, new ResultResponse(BL_Response.CorruptedData, "Corrupted Data"));
            }
            string? comment = found.Confirmed ? found.Comment : null;

            var returnValue = new Rating(found.Id, user.Value, media.Value, comment, found.Stars, found.TimeStamp);
            return new Result<Rating>(returnValue, new ResultResponse(BL_Response.OK, null));
        }

        public async Task<ResultResponse> ConfirmComment(string authenticationToken, int ratingId)
        {
            var user = await _userService.AuthenticateUserByToken(authenticationToken);
            if (user.Value == null)
            {
                return user.Response;
            }
            var rating = await FindRatingById(ratingId);
            if (rating.Value == null)
            {
                return rating.Response;
            }
            if (user.Value.Username != rating.Value.User.Username)
            {
                return new ResultResponse(BL_Response.Unauthorized, "Not the author of this rating");
            }
            if (!await _dal.RatingRepo.UpdateRating(new UpdateRating(ratingId,null, null, true)))
            {
                return new ResultResponse(BL_Response.InternalError, "Could not confirm the comment");
            }
            return new ResultResponse(BL_Response.OK, "Comment comfirmed");
        }

        public async Task<ResultResponse> LikeRating(string authenticationToken, int ratingId)
        {
            var user = await _userService.AuthenticateUserByToken(authenticationToken);
            if (user.Value == null)
            {
                return user.Response;
            }
            if (!await _dal.RatingRepo.LikeRating(user.Value.Username, ratingId))
            {
                return new ResultResponse(BL_Response.InternalError, "Couldnt Like Rating");
            }
            await _dal.UserRepo.AddActivityPoints(user.Value.Username, 1);
            return new ResultResponse(BL_Response.OK, null);
        }
    }
}
