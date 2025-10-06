using BusinessLogic;

namespace API
{
    public class RatingHandler : Controller, IRatingHandler
    {
        public RatingHandler(IBL bl) : base(bl)
        {
        }
    }
}