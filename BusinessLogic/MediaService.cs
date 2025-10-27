using BusinessLogic.Models;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class MediaService:IMediaService
    {
        private IDAL _dal;
        public MediaService(IDAL dal) {
            _dal = dal;
        }
        public async Task<Media?> FindMediaById(int id)
        {
            var found = await _dal.MediaRepo.FindMediaById(id);
            if (found == null)
            {
                return null;
            }
            return new Media(found.Id, found.Title, found.Description, found.ReleaseDate, found.Fsk, found.Genres, found.MediaType);
        }
    }
}
