using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public interface IMediaRepository
    {

        public Task<MediaEntity?> FindMediaById(int id);
        public Task<List<MediaEntity>> GetMedia();
        public Task<List<GenreEntity>> GetGenres();
        public Task<GenreEntity?> FindGenreById(int id);
        public Task<List<MediaTypeEntity>> GetMediaTypes();
        public Task<int?> AddMedia(AddMedia media);
        public Task<bool> UpdateMedia(UpdateMedia media);
        public Task<bool> DeleteMedia(int id);

    }
}
