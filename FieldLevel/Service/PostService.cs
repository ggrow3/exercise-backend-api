using FieldLevel.Controllers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace FieldLevel.Service
{
    public class PostService : IPostService
    {

        private readonly IConfiguration _config;

        public PostService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<IEnumerable<Post>> GetPosts()
        {
            var httpClient = new HttpClient();
            var uri = this._config.GetValue<string>("Posts:uri");
          
            using var httpResponse = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);

            httpResponse.EnsureSuccessStatusCode();

            if (httpResponse.Content is object && httpResponse.Content.Headers.ContentType.MediaType == "application/json")
            {
                var contentStream = await httpResponse.Content.ReadAsStreamAsync();

                return await System.Text.Json.JsonSerializer.DeserializeAsync<IEnumerable<Post>>(contentStream, new System.Text.Json.JsonSerializerOptions { IgnoreNullValues = true, PropertyNameCaseInsensitive = true });
            }
            else
            {
                throw new Exception("HTTP Response was invalid and cannot be deserialized.");
            }
        }

        public async Task<IEnumerable<Post>> GetUsersLastPost()
        {
            var posts = await this.GetPosts();
            posts = posts.GroupBy(post => post.userId)
                             .Select(post => post.OrderByDescending(post => post.id)
                                                 .Take(1))
                             .SelectMany(post => post);

            return posts;
        }
    }
}
