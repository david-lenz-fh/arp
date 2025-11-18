using BusinessLogic.Models;

namespace BusinessLogic
{
    public interface IMediaService
    {
        public Task<Result<Media>> FindMediaById(int id);
        public Task<Result<List<Media>>> GetMedia(MediaFilter? filter);
        public Task<ResultResponse> DeleteMediaById(string authenticationToken, int id);
        public Task<Result<int?>> PostMedia(string authenticationToken, PostMedia newMedia);
        public Task<ResultResponse> PutMedia(string authenticationToken, PutMedia putMedia);
    }
}