using Castle.Core.Logging;
using Google.Apis.Compute.v1.Data;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using NTChallengeBackendWebApi.Helpers;
using NTChallengeBackendWebApi.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NTChallengeTestProject
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public async Task When_CallGetLatestStories_Returns_Stor()
        {
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            var logger = new Mock<ILogger<HackerNewsService>>();
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddMemoryCache();
            var serviceProvider = services.BuildServiceProvider();

            var memCache = serviceProvider.GetService<IMemoryCache>();

            HttpResponseMessage httpResponseMessage = new()
            {
                Content = JsonContent.Create(new
                {
                    title = "",
                    url = ""
                })
            };

            httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(httpResponseMessage);

            var httpClient = new HttpClient(httpMessageHandlerMock.Object)
            {
                BaseAddress = new System.Uri("https://localhost:44324/News/GetLatestStories")
            };

            HackerNewsService hackerNewsService = new HackerNewsService(memCache, logger.Object);

            //Act
            var res = await hackerNewsService.GetLatestStories();

            //Assert
            Assert.AreEqual(typeof(List<StoryModel>), res.GetType());
        }

        
    }
}
