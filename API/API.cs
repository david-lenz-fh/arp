﻿using BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public class API:IAPI
    {

        public IUserHandler UserHandler { get; }
        public IMediaHandler MediaHandler { get; }
        public IRatingHandler RatingHandler { get; }
        public API(IUserHandler userHandler, IMediaHandler mediaHandler, IRatingHandler ratingHandler)
        {
            UserHandler = userHandler;
            MediaHandler = mediaHandler;
            RatingHandler = ratingHandler;
        }
    }
}
