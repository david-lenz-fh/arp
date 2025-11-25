using BusinessLogic.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public interface IBL
    {
        public IUserService UserService { get; }
        public IMediaService MediaService { get; }
        public IRatingService RatingService { get; }
        public IRecommendationService RecommendationService { get; }
    }
}

