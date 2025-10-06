using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public interface IDAL
    {
        public IMediaRepository MediaRepo { get; }
        public IUserRepository UserRepo { get; }
        public IRatingRepository RatingRepo { get; }
    }
}
