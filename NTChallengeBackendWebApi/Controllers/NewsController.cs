using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NTChallengeBackendWebApi.Helpers;
using NTChallengeBackendWebApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NTChallengeBackendWebApi.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    [ApiController]
    [Route("[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly ILogger _appInsightsLogger;
        private HackerNewsService _hackerNewsService;

        public NewsController(ILogger<NewsController> logger, HackerNewsService hackerNewsService)
        {
            if (logger != null)
                _appInsightsLogger = logger;
            else { throw new ArgumentNullException("Logger was null"); }


            if (hackerNewsService != null)
                _hackerNewsService = hackerNewsService;
            else { _appInsightsLogger.LogError("HackerNewsService was null"); 
                throw new ArgumentNullException("HackerNewsService was null"); }
        }

        // GET: api/<NewsController>
        [HttpGet]
        [Route("GetLatestStories")]
        public async Task<List<StoryModel>> GetLatestStories()
        {
            if (_hackerNewsService != null)
                return await _hackerNewsService.GetLatestStories();
            else return null;
        }

    }
}
