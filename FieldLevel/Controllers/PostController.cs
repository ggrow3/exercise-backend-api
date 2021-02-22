using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FieldLevel.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly ILogger<PostController> _logger;
        private readonly IMemoryCache _cache;
        private readonly IPostService _postService;
        private readonly IConfiguration _config;
        private const string PostsCacheKey = "GetUsersLastPost";

        public PostController(IConfiguration config, ILogger<PostController> logger, IMemoryCache memoryCache, IPostService postService)
        {
            _config = config;
            _logger = logger;
            _cache = memoryCache;
            _postService = postService;
        }

        [HttpGet]
        public async Task<IEnumerable<Post>> Get()
        {
            if (_cache.TryGetValue(PostsCacheKey, out IEnumerable<Post> posts))
            {
                return posts;
            }

            posts = await _postService.GetUsersLastPost();

            var cacheTime = this._config.GetValue<int>("Posts:CacheTime");
            _cache.Set(PostsCacheKey, posts, TimeSpan.FromSeconds(cacheTime));
            _logger.LogInformation($"Retrieved {posts.Count()}");

            return posts;
        }

       
    }
}
