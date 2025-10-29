using BusinessLogic.Models;

namespace BusinessLogic
{
    public interface IMediaService
    {
        public Task<Media?> FindMediaById(int id);
        public Task<List<Media>> GetMedia(MediaFilter filter);
        public Task<bool> DeleteMediaById(int id);
    }
}