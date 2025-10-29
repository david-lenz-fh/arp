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
        public Task<List<MediaEntity>> GetMedia(MediaFilterDAL filter);
        public Task<List<string>> GetGenres();
        public Task<int?> AddMedia(AddMedia media);
        public Task<bool> UpdateMedia(UpdateMedia media);
        public Task<bool> DeleteMedia(int id);

    }
}
