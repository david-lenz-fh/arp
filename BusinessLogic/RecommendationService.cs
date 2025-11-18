using BusinessLogic.Models;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    namespace BusinessLogic
    {
        public class RecommendationService : IRecommendationService
        {
            private IDAL _dal;
            private IUserService _userService;
            private IMediaService _mediaService;
            private IRatingService _ratingService;
            public RecommendationService(IDAL dal, IMediaService mediaService, IUserService userService, IRatingService ratingService)
            {
                _dal = dal;
                _userService = userService;
                _mediaService = mediaService;
                _ratingService = ratingService;
            }
            
            public async Task<Result<List<(Media, decimal)>>> GetRecommendations(string authenticationToken)
            {
                List<(Media, decimal)> recommendationScores = new List<(Media, decimal)>();
                var user = await _userService.AuthenticateUserByToken(authenticationToken);
                if (user.Value == null)
                {
                    return new Result<List<(Media, decimal)>>(null, user.Response);
                }
                var mediaResults = await _mediaService.GetMedia(null);
                if(mediaResults.Value == null)
                {
                    return new Result<List<(Media, decimal)>>(null, mediaResults.Response);
                }
                List<Media> allMedia = mediaResults.Value;

                var ratingsResult = await _ratingService.GetRatingsFromUser(user.Value.Username, null);
                if (ratingsResult.Value == null)
                {
                    return new Result<List<(Media, decimal)>>(null, ratingsResult.Response);
                }

                List<Media> unratedMedia = allMedia.Where(rm => !ratingsResult.Value.Select(r => r.Id).Contains(rm.Id)).ToList();
                foreach (var media in unratedMedia)
                {
                    List<decimal> similarityScores = new List<decimal>();
                    foreach (var rating in ratingsResult.Value)
                    {
                        int ratingCutoff = 3;
                        if (rating.Stars < ratingCutoff)
                        {
                            continue;
                        }
                        similarityScores.Add(GetSimilarityScore(media, rating.Media));
                    }
                    decimal score = similarityScores.Average();
                    if (user.Value.FavouriteGenre != null && media.Genres.Contains(user.Value.FavouriteGenre))
                    {
                        score = score * 0.8m + 0.2m;
                    }
                    recommendationScores.Add((media, score));
                }
                return new Result<List<(Media, decimal)>>(recommendationScores, new ResultResponse(BL_Response.OK, null));
            }
            private decimal GetSimilarityScore(Media media1, Media media2)
            {
                int amountSameGenres = media1.Genres.Intersect(media2.Genres).Count();
                int amountGenres = media1.Genres.Union(media2.Genres).Count();
                decimal genreSimiliarity = amountSameGenres / amountGenres;

                decimal starSimiliarity = 0;
                if (media1.AverageStars != null && media2.AverageStars != null)
                {
                    starSimiliarity = (5 - Math.Abs(media1.AverageStars.Value - media2.AverageStars.Value)) / 5;
                }
                decimal releaseSimiliarity = 0;
                if (media1.ReleaseDate != null && media2.ReleaseDate != null)
                {
                    int yearDifference = Math.Abs(media1.ReleaseDate.Value.Year - media2.ReleaseDate.Value.Year);
                    if (yearDifference < 10)
                    {
                        releaseSimiliarity = (10 - yearDifference) / 10;
                    }
                }

                decimal sameMediaType = 0;
                if (media1.MediaType != null && media2.MediaType != null && media1.MediaType == media2.MediaType) {
                    sameMediaType = 1;
                }
                return (genreSimiliarity + starSimiliarity + releaseSimiliarity + sameMediaType) / 4;
            }
        }
    }
}
