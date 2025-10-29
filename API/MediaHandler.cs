using BusinessLogic;
using BusinessLogic.Models;
using System.Net;

namespace API
{
    public class MediaHandler:Controller, IMediaHandler
    {
        public MediaHandler(IBL bl):base(bl)
        {
            
        }

        public async Task GetMedia(HttpListenerContext ctx)
        {
            Dictionary<string, string> qp = GetQueryParams(ctx);
            string? fskQuery = qp.GetValueOrDefault("ageRestriction");
            int? fsk = null;
            if (fskQuery != null && int.TryParse(fskQuery, out int val)) {
                fsk = val;
            }
            string? releaseYearQuery = qp.GetValueOrDefault("releaseYear");
            int? releaseYear = null;
            if (releaseYearQuery != null && int.TryParse(releaseYearQuery, out val))
            {
                releaseYear = val;
            }
            string? ratingQuery = qp.GetValueOrDefault("rating");
            int? rating = null;
            if (ratingQuery != null && int.TryParse(ratingQuery, out val))
            {
                rating = val;
            }
            var filter = new MediaFilter(qp.GetValueOrDefault("title"), qp.GetValueOrDefault("mediaType"), releaseYear, qp.GetValueOrDefault("genre"), fsk, rating, qp.GetValueOrDefault("sortBy"));

            var medias = new List<MediaDTO>();
            var foundMedias = await _bl.MediaService.GetMedia(filter);
            foreach (var found in foundMedias)
            {
                medias.Add(new MediaDTO(found.Id, found.Title, found.Description, found.AverageRating, found.ReleaseDate, found.Fsk, found.Genres, found.MediaType));
            }
            WriteJson(ctx, medias);
        }
        public async Task FindMediaById(HttpListenerContext ctx, Dictionary<string,string> parameters)
        {
            string? mediaIdString = parameters.GetValueOrDefault("mediaId");
            if(mediaIdString == null||!int.TryParse(mediaIdString, out int mediaId))
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No valid mediaId was sent");
                return;
            }
            var found = await _bl.MediaService.FindMediaById(mediaId);
            if (found==null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.NotFound, String.Format("Media with the ID: {0} not found", mediaId));
                return;
            }
            WriteJson(ctx, found);
        }
    }
}