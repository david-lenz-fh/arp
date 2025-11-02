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
            if (foundMedias.Value == null)
            {
                SendResultResponse(ctx,foundMedias.Response);
                return;
            }
            foreach (var found in foundMedias.Value)
            {
                medias.Add(new MediaDTO(found.Id, found.Title, found.Description, found.AverageStars, found.ReleaseDate, found.Fsk, found.Genres, found.MediaType, found.Creator.Username));
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
            var result = await _bl.MediaService.FindMediaById(mediaId);
            if (result.Value==null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.NotFound, String.Format("Media with the ID: {0} not found", mediaId));
                return;
            }
            var found = result.Value;
            var media = new MediaDTO(found.Id, found.Title, found.Description, found.AverageStars, found.ReleaseDate, found.Fsk, found.Genres, found.MediaType, found.Creator.Username);
            WriteJson(ctx, media);
        }

        public async Task DeleteMediaById(HttpListenerContext ctx, Dictionary<string, string> parameters)
        {
            string? mediaIdString = parameters.GetValueOrDefault("mediaId");
            if (mediaIdString == null || !int.TryParse(mediaIdString, out int mediaId))
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No valid mediaId was sent");
                return;
            }
            string? token = ReadBearerToken(ctx);
            if (token == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.Unauthorized, "No Authentication was send (Bearer Token)");
                return;
            }
            SendResultResponse(ctx, await _bl.MediaService.DeleteMediaById(token, mediaId));
        }

        public async Task PostMedia(HttpListenerContext ctx)
        {
            var postedMedia=await ReadJSONRequestAsync<PostMediaDTO>(ctx);
            if (postedMedia == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.BadRequest, "No valid media was sent");
                return;
            }
            var token = ReadBearerToken(ctx);
            if (token == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.Unauthorized, "Authentication required for creating media");
                return;
            }
            DateOnly? release_date = null;
            if(DateOnly.TryParse(postedMedia.ReleaseYear + "-01-01", out DateOnly val)){
                release_date = val;
            }
            var newId=await _bl.MediaService.PostMedia(token, new PostMedia(postedMedia.Title, postedMedia.Description, release_date,
                postedMedia.AgeRestriction, postedMedia.Genres ?? new List<string>(), postedMedia.MediaType));
            if (newId.Value == null)
            {
                SendResultResponse(ctx, newId.Response);
                return;
            }
            WriteJson(ctx, new { MediaId = newId.Value});
        }
        public async Task PutMedia(HttpListenerContext ctx, Dictionary<string,string> parameters)
        {
            var token = ReadBearerToken(ctx);
            if (token == null)
            {
                SendEmptyStatus(ctx, HttpStatusCode.Unauthorized, "Authentication required for creating media");
                return;
            }
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
            SendResultResponse(ctx, await _bl.MediaService.PutMedia(token, new PutMedia(
                mediaId, putMedia.Title, putMedia.Description, release_date, putMedia.AgeRestriction, putMedia.Genres, putMedia.MediaType)
            ));
        }
    }
}