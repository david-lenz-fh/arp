using BusinessLogic;
using BusinessLogic.Models;
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
                ratings.Add(new RatingDTO(found.Id, found.User.Username, new RatingMediaDTO(found.Media.Id, found.Media.Title??""), found.Comment, found.Rating));
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


    }
}