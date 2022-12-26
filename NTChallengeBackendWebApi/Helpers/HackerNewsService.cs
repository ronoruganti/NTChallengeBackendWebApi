using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NTChallengeBackendWebApi.Controllers;
using NTChallengeBackendWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NTChallengeBackendWebApi.Helpers
{
    public class HackerNewsService
    {
        private IMemoryCache _cache;
        private readonly ILogger _appInsightsLogger;
        HttpClient client = new HttpClient();
        string latestStoriesUrl = "https://hacker-news.firebaseio.com/v0/newstories.json";
        string getStoryByIDUrl = "https://hacker-news.firebaseio.com/v0/item/";

        public HackerNewsService(IMemoryCache cache, ILogger<HackerNewsService> logger)
        {
            if (logger != null)
                _appInsightsLogger = logger;
            else { throw new ArgumentNullException("Logger was null"); }

            if (cache != null)
                _cache = cache;
            else
            {
                _appInsightsLogger.LogError("Cache was null");
                throw new ArgumentNullException("Cache was null");
            }
        }

        public async Task<List<StoryModel>> GetLatestStories()
        {
            var latestStories = new List<StoryModel>();
            try
            {
                var storyIDs = await GetLatestStoryIDsAsync();

                storyIDs.ForEach(async n =>
                {
                    var story = await GetStoryByID(n);
                    latestStories.Add(story);
                });
            }
            catch (Exception)
            {
                 _appInsightsLogger.LogError("Error occurred while getting stories");
                throw;
            }

            return latestStories;
        }

        private async Task<List<int>> GetLatestStoryIDsAsync()
        {
            List<int> storyIDs = null;

            try
            {
                HttpResponseMessage response = await client.GetAsync(latestStoriesUrl);

                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsStringAsync();
                    storyIDs = res.Replace("[", "").Replace("]", "").Split(",").Select(x => Int32.Parse(x)).ToList();
                }
            }
            catch (Exception)
            {
                _appInsightsLogger.LogError("Error occurred while getting story IDs");
                throw;
            }

            return storyIDs;
        }

        private async Task<StoryModel> GetStoryByID(int storyID)
        {
            try
            {
                return await _cache.GetOrCreateAsync(storyID, async i => {
                    StoryModel story = new StoryModel();

                    HttpResponseMessage response = await client.GetAsync($"{getStoryByIDUrl}{storyID}.json");
                    if (response.IsSuccessStatusCode)
                    {
                        var res = await response.Content.ReadAsStringAsync();
                        story = JsonConvert.DeserializeObject<StoryModel>(res);
                    }

                    return story;
                });
            }
            catch (Exception)
            {
                _appInsightsLogger.LogError("Error occurred while getting individual story");
                throw;
            }                        
        }
    }
}
