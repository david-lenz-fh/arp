using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class RatingService:IRatingService
    {
        private IDAL _dal;
        public RatingService(IDAL dal)
        {
            _dal = dal;
        }
    }
}
