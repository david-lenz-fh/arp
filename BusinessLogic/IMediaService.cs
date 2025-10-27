using BusinessLogic.Models;

namespace BusinessLogic
{
    public interface IMediaService
    {
        public Task<Media?> FindMediaById(int id);
    }
}