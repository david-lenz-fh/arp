using BusinessLogic;
using BusinessLogic.Models;
using DataAccess.Entities;
using System.Net;
using System.Web;

namespace API
{
    public class RatingHandler : Controller, IRatingHandler
    {
        public RatingHandler(IBL bl) : base(bl)
        {
        }
        public async Task GetUserRatings(HttpListenerContext ctx, Dictionary<string, string> parameters)
        {
            string? username = parameters.GetValueOrDefault("userId");
            if (username == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No User Id");
                return;
            }
            User? foundUser = await _bl.UserService.FindUserByName(username);
            if (foundUser == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.NotFound, String.Format("User \"{0}\" not found", username));
                return;
            }
            var foundRatings = await _bl.RatingService.GetReviewsFromUser(foundUser);
            var ratings = new List<RatingDTO>();
            foreach (var found in foundRatings)
            {
                ratings.Add(new RatingDTO(found.Id, found.User.Username, new RatingMediaDTO(found.Media.Id, found.Media.Title??""), found.Comment, found.Stars));
            }
            if (ratings.Count == 0) {
                SendEmptyStatus(ctx, HttpStatusCode.NotFound, "No User Reviews found");
                return;
            }
            WriteJson(ctx, ratings);
        }
        public async Task GetUserFavourites(HttpListenerContext ctx, Dictionary<string, string> parameters)
        {
            string? username = parameters.GetValueOrDefault("userId");
            if (username == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No User Id");
                return;
            }
            User? foundUser = await _bl.UserService.FindUserByName(username);
            if (foundUser == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.NotFound, String.Format("User \"{0}\" not found", username));
                return;
            }

            var foundFavourites=await _bl.RatingService.GetFavouritesFromUser(foundUser);
            var favourites = new List<FavouriteDTO>();
            foreach (var found in foundFavourites)
            {
                favourites.Add(new FavouriteDTO(found.user.Username,
                    new FavouriteMediaDTO(found.media.Id, found.media.Title, found.media.ReleaseDate, found.media.MediaType)));
            }
            if (favourites.Count == 0)
            {
                SendEmptyStatus(ctx, HttpStatusCode.NotFound, "No User Favourites found");
                return;
            }
            WriteJson(ctx, favourites);
        }
        public async Task PostRating(HttpListenerContext ctx, Dictionary<string,string> parameters)
        {
            string? mediaIdString = parameters.GetValueOrDefault("mediaId");
            if (mediaIdString == null || !int.TryParse(mediaIdString, out int mediaId))
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No MediaId");
                return;
            }
            var postedRating = await ReadJSONRequestAsync<AddRatingDTO>(ctx);
            if (postedRating == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "Wrong Request JSON");
                return;
            }
            Token? token=ReadBearerToken(ctx);
            if (token == null) {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No Token was send");
                return;
            }
            User? user=await _bl.UserService.GetUserFromToken(token);
            if (user == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.Unauthorized, "No user with this authentication found");
                return;
            }
            Media? media=await _bl.MediaService.FindMediaById(mediaId);
            if (media == null) {
                SendEmptyStatus(ctx, HttpStatusCode.NotFound, "No Media with this Id");
                return;
            }
            int? id = await _bl.RatingService.PostRating(new PostRating(user, media, postedRating.Comment, postedRating.Stars));
            if (id == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.InternalServerError, "Couldnt add rating");
                return;
            }
            WriteJson(ctx, new {ratingId=id});
        }

        public async Task Favourite(HttpListenerContext ctx, Dictionary<string,string> parameters)
        {
            string? mediaIdString = parameters.GetValueOrDefault("mediaId");
            if (mediaIdString == null || !int.TryParse(mediaIdString, out int mediaId))
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No MediaId");
                return;
            }
            Token? token = ReadBearerToken(ctx);
            if (token == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No Token was send");
                return;
            }
            User? user = await _bl.UserService.GetUserFromToken(token);
            if (user == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.Unauthorized, "No user with this authentication found");
                return;
            }
            Media? media = await _bl.MediaService.FindMediaById(mediaId);
            if (media == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.NotFound, "No Media with this Id");
                return;
            }
            if (!await _bl.RatingService.Favourite(user, media))
            {
                SendEmptyStatus(ctx, HttpStatusCode.InternalServerError, "Couldnt favourite");
                return;
            }
            SendEmptyStatus(ctx, HttpStatusCode.OK, "Favourited");
        }
        public async Task Unfavourite(HttpListenerContext ctx, Dictionary<string, string> parameters)
        {
            string? mediaIdString = parameters.GetValueOrDefault("mediaId");
            if (mediaIdString == null || !int.TryParse(mediaIdString, out int mediaId))
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No MediaId");
                return;
            }
            Token? token = ReadBearerToken(ctx);
            if (token == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No Token was send");
                return;
            }
            User? user = await _bl.UserService.GetUserFromToken(token);
            if (user == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.Unauthorized, "No user with this authentication found");
                return;
            }
            Media? media = await _bl.MediaService.FindMediaById(mediaId);
            if (media == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.NotFound, "No Media with this Id");
                return;
            }
            if(!await _bl.RatingService.Unfavourite(user, media))
            {
                SendEmptyStatus(ctx, HttpStatusCode.InternalServerError, "Couldnt unfavourite");
                return;
            }
            SendEmptyStatus(ctx, HttpStatusCode.OK, "Unfavourited");
        }
    }
}