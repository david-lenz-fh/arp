using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DAL(IMediaRepository mediaRepo, IUserRepository userRepo, IRatingRepository ratingRepo) : IDAL
    {
        public IMediaRepository MediaRepo { get; } = mediaRepo;
        public IUserRepository UserRepo { get; } = userRepo;
        public IRatingRepository RatingRepo { get; } = ratingRepo;
    }
}
