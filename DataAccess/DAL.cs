using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DAL:IDAL
    {
        public IMediaRepository MediaRepo { get; }
        public IUserRepository UserRepo { get; }
        public IRatingRepository RatingRepo { get; }

        public DAL(IMediaRepository mediaRepo, IUserRepository userRepo, IRatingRepository ratingRepo)
        {
            MediaRepo = mediaRepo;
            UserRepo = userRepo;
            RatingRepo = ratingRepo;
        }
    }
}
