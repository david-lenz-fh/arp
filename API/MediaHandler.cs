using BusinessLogic;

namespace API
{
    public class MediaHandler:Controller, IMediaHandler
    {
        public MediaHandler(IBL bl):base(bl)
        {
            
        }
    }
}