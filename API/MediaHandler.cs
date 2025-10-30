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
                medias.Add(new MediaDTO(found.Id, found.Title, found.Description, found.AverageStars, found.ReleaseDate, found.Fsk, found.Genres, found.MediaType));
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

        public async Task DeleteMediaById(HttpListenerContext ctx, Dictionary<string, string> parameters)
        {
            string? mediaIdString = parameters.GetValueOrDefault("mediaId");
            if (mediaIdString == null || !int.TryParse(mediaIdString, out int mediaId))
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No valid mediaId was sent");
                return;
            }
            if (!await _bl.MediaService.DeleteMediaById(mediaId))
            {
                SendEmptyStatus(ctx, HttpStatusCode.NotFound, String.Format("Media with the ID: {0} could not be deleted", mediaId));
                return;
            }
            SendEmptyStatus(ctx, HttpStatusCode.OK, String.Format("Media with the ID: {0} was deleted", mediaId));
        }

        public async Task PostMedia(HttpListenerContext ctx)
        {
            var postedMedia=await ReadJSONRequestAsync<PostMediaDTO>(ctx);
            if (postedMedia == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No valid media was sent");
                return;
            }
            DateOnly? release_date = null;
            if(DateOnly.TryParse(postedMedia.ReleaseYear + "-01-01", out DateOnly val)){
                release_date = val;
            }
            int? newId=await _bl.MediaService.PostMedia(new PostMedia(postedMedia.Title, postedMedia.Description, release_date,
                postedMedia.AgeRestriction, postedMedia.Genres ?? new List<string>(), postedMedia.MediaType));
            if (newId == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.InternalServerError, "Couldnt add media");
                return;
            }
            WriteJson(ctx, new { MediaId = newId});
        }
        public async Task PutMedia(HttpListenerContext ctx, Dictionary<string,string> parameters)
        {
            string? mediaIdString = parameters.GetValueOrDefault("mediaId");
            if (mediaIdString == null||!int.TryParse(mediaIdString, out int mediaId))
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No MediaId");
                return;
            }
            var putMedia = await ReadJSONRequestAsync<PostMediaDTO>(ctx);
            if (putMedia == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No valid media was sent");
                return;
            }
            DateOnly? release_date = null;
            if (DateOnly.TryParse(putMedia.ReleaseYear + "-01-01", out DateOnly val))
            {
                release_date = val;
            }

            if(!await _bl.MediaService.PutMedia(
                new PutMedia(mediaId, putMedia.Title, putMedia.Description, release_date, putMedia.AgeRestriction, putMedia.Genres, putMedia.MediaType)))
            {
                SendEmptyStatus(ctx, HttpStatusCode.InternalServerError, "Couldnt change media");
                return;
            }
            SendEmptyStatus(ctx, HttpStatusCode.OK, "Media was changed");
        }
    }
}