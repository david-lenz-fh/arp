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
    }
}
