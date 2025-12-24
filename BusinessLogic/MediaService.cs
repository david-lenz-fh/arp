using BusinessLogic.Models;
using DataAccess;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class MediaService:IMediaService
    {
        private IDAL _dal;
        private IUserService _userService;
        public MediaService(IDAL dal, IUserService userService) {
            _dal = dal;
            _userService = userService;
        }
        public async Task<Result<Media>> FindMediaById(int id)
        {
            var found = await _dal.MediaRepo.FindMediaById(id);
            if (found == null)
            {
                return new Result<Media>(null, new ResultResponse(BL_Response.NotFound, "Media not found"));
            }
            var foundUser = await _dal.UserRepo.FindUserByName(found.CreatorName);
            if(foundUser == null)
            {
                return new Result<Media>(null, new ResultResponse(BL_Response.CorruptedData, "Corrupted Data")); ;
            }
            var user = new User(foundUser.Username, foundUser.Password, foundUser.Email, foundUser.FavouriteGenre);
            var returnValue=new Media(found.Id, found.Title, found.Description, found.ReleaseDate, found.Fsk, found.Genres, found.MediaType, found.AverageStars, user);
            return new Result<Media>(returnValue, new ResultResponse(BL_Response.OK, "Media found"));
        }

        public async Task<ResultResponse> DeleteMediaById(string authenticationToken, int id)
        {
            var user = await _userService.AuthenticateUserByToken(authenticationToken);
            if (user.Value == null)
            {
                return user.Response;
            }
            var media = await FindMediaById(id);
            if (media.Value == null)
            {
                return media.Response;
            }
            if (media.Value.Creator.Username != user.Value.Username)
            {
                return new ResultResponse(BL_Response.Unauthorized, "Authenticated User is not the creator of that media");
            }
            if(!await _dal.MediaRepo.DeleteMedia(id))
            {
                return new ResultResponse(BL_Response.InternalError, "Couldn´t delete´media");
            }
            return new ResultResponse(BL_Response.OK, "Media deleted");
        }
        public async Task<Result<List<Media>>> GetMedia(MediaFilter? filter)
        {
            MediaFilterDAL filterDAL = filter == null ? new MediaFilterDAL(null,null,null,null,null,null,null) :
                new MediaFilterDAL(filter.Title, filter.MediaType, filter.ReleaseYear, filter.Genre, filter.Fsk, filter.MinStars, filter.SortBy);


            var returnValue =new List<Media>();
            var foundMedias = await _dal.MediaRepo.GetMedia(filterDAL);

            foreach (var found in foundMedias)
            {
                var foundUser = await _dal.UserRepo.FindUserByName(found.CreatorName);
                if (foundUser == null)
                {
                    continue;
                }
                var user = new User(foundUser.Username, foundUser.Password, foundUser.Email, foundUser.FavouriteGenre);
                returnValue.Add(new Media(found.Id, found.Title, found.Description, found.ReleaseDate, found.Fsk, found.Genres, found.MediaType, found.AverageStars, user));
            }
            return new Result<List<Media>>(returnValue, new ResultResponse(BL_Response.OK, null));
        }

        public async Task<Result<int?>> PostMedia(string authenticationToken, PostMedia postedMedia)
        {
            var user = await _userService.AuthenticateUserByToken(authenticationToken);
            if (user.Value == null)
            {
                return new Result<int?>(null, user.Response);
            }
            if(postedMedia.MediaType!="Series" && postedMedia.MediaType != "Movie" && postedMedia.MediaType != "Game")
            {
                return new Result<int?>(null, new ResultResponse(BL_Response.BadParameters, "Media Type must be either of the following: Series, Game, Movie"));
            }
            var addMedia = new AddMedia(postedMedia.Title, postedMedia.Description, postedMedia.ReleaseDate, postedMedia.Fsk, postedMedia.Genres, postedMedia.MediaType, user.Value.Username);
            var returnValue = await _dal.MediaRepo.AddMedia(addMedia);
            if (returnValue == null)
            {
                return new Result<int?>(null, new ResultResponse(BL_Response.NotFound, "Resource not found"));
            }
            await _dal.UserRepo.AddActivityPoints(user.Value.Username, 3);
            return new Result<int?>(returnValue, new ResultResponse(BL_Response.OK, "Media created"));
        }
        public async Task<ResultResponse> PutMedia(string authenticationToken, PutMedia putMedia)
        {
            var user = await _userService.AuthenticateUserByToken(authenticationToken);
            if (user.Value == null)
            {
                return user.Response;
            }
            var media = await FindMediaById(putMedia.Id);
            if (media.Value==null)
            {
                return media.Response;
            }
            if (media.Value.Creator.Username != user.Value.Username)
            {
                return new ResultResponse(BL_Response.Unauthorized, "Authenticated User is not the creator of that media");
            }
            var updateMedia = new UpdateMedia(putMedia.Id, putMedia.Title, putMedia.Description, putMedia.ReleaseDate, putMedia.Fsk, putMedia.Genres, putMedia.MediaType);
            if(!await _dal.MediaRepo.UpdateMedia(updateMedia))
            {
                return new ResultResponse(BL_Response.InternalError, "Couldnt update media");
            }
            await _dal.UserRepo.AddActivityPoints(user.Value.Username, 1);
            return new ResultResponse(BL_Response.OK, "Media changed");
        }
    }
}
