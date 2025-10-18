using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public interface IAPI
    {

        public IUserHandler UserHandler { get; }
        public IMediaHandler MediaHandler { get; }
        public IRatingHandler RatingHandler { get; }

        
    }
}
