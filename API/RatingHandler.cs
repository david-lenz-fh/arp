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
            string? token = ReadBearerToken(ctx);
            var foundRatings = await _bl.RatingService.GetRatingsFromUser(username, token);
            var ratings = new List<RatingDTO>();
            if (foundRatings.Value == null)
            {
                SendResultResponse(ctx, foundRatings.Response);
                return;
            }
            foreach (var found in foundRatings.Value)
            {
                ratings.Add(new RatingDTO(found.Id, found.User.Username, new RatingMediaDTO(found.Media.Id, found.Media.Title??""), found.Comment, found.Stars, found.Timestamp));
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
            var foundFavourites=await _bl.RatingService.GetFavouritesFromUser(username);
            if (foundFavourites.Value == null)
            {
                SendResultResponse(ctx, foundFavourites.Response);
                return;
            }
            var favourites = new List<FavouriteDTO>();
            foreach (var found in foundFavourites.Value)
            {
                favourites.Add(new FavouriteDTO(found.user.Username,
                    new FavouriteMediaDTO(found.media.Id, found.media.Title, found.media.ReleaseDate, found.media.MediaType)));
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
            string? token=ReadBearerToken(ctx);
            if (token == null) {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No Token was send");
                return;
            }
            var postedRating = await ReadJSONRequestAsync<AddRatingDTO>(ctx);
            if (postedRating == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "Wrong Request JSON");
                return;
            }
            if (postedRating.Stars == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No Stars in Rating");
                return;
            }
            var id = await _bl.RatingService.PostRating(token, new PostRating(mediaId, postedRating.Comment, postedRating.Stars.Value));
            if (id.Value == null)
            {
                SendResultResponse(ctx, id.Response);
                return;
            }
            WriteJson(ctx, new {ratingId=id.Value});
        }
        public async Task DeleteRating(HttpListenerContext ctx, Dictionary<string, string> parameters)
        {

            string? ratingIdString = parameters.GetValueOrDefault("ratingId");
            if (ratingIdString == null || !int.TryParse(ratingIdString, out int ratingId))
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No MediaId");
                return;
            }
            string? token = ReadBearerToken(ctx);
            if (token == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No Token was send");
                return;
            }
            SendResultResponse(ctx, await _bl.RatingService.DeleteRatingById(token, ratingId));
        }
        public async Task Favourite(HttpListenerContext ctx, Dictionary<string,string> parameters)
        {
            string? mediaIdString = parameters.GetValueOrDefault("mediaId");
            if (mediaIdString == null || !int.TryParse(mediaIdString, out int mediaId))
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No MediaId");
                return;
            }
            string? token = ReadBearerToken(ctx);
            if (token == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No Token was send");
                return;
            }
            SendResultResponse(ctx, await _bl.RatingService.Favourite(token, mediaId));
        }
        public async Task Unfavourite(HttpListenerContext ctx, Dictionary<string, string> parameters)
        {
            string? mediaIdString = parameters.GetValueOrDefault("mediaId");
            if (mediaIdString == null || !int.TryParse(mediaIdString, out int mediaId))
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No MediaId");
                return;
            }
            string? token = ReadBearerToken(ctx);
            if (token == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No Token was send");
                return;
            }
            SendResultResponse(ctx, await _bl.RatingService.Unfavourite(token, mediaId));
        }

        public async Task PutRating(HttpListenerContext ctx, Dictionary<string, string> parameters)
        {
            string? ratingIdString = parameters.GetValueOrDefault("ratingId");
            if (ratingIdString == null || !int.TryParse(ratingIdString, out int ratingId))
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No ratingId");
                return;
            }

            string? token = ReadBearerToken(ctx);
            if (token == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No Token was send");
                return;
            }
            var putRating = await ReadJSONRequestAsync<AddRatingDTO>(ctx);
            if (putRating == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No Request Body");
                return;
            }
            SendResultResponse(ctx, await _bl.RatingService.PutRating(token, new PutRating(ratingId, putRating.Comment, putRating.Stars)));
        }

        public async Task ConfirmComment(HttpListenerContext ctx, Dictionary<string, string> parameters)
        {
            string? ratingIdString = parameters.GetValueOrDefault("ratingId");
            if (ratingIdString == null || !int.TryParse(ratingIdString, out int ratingId))
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No Rating ID");
                return;
            }
            string? token = ReadBearerToken(ctx);
            if (token == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No Token was send");
                return;
            }
            SendResultResponse(ctx, await _bl.RatingService.ConfirmComment(token, ratingId));
        }
    }
}