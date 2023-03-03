using ClientImport.DbContexts;
using ClientImport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using ClientImport.DTOs;
using Microsoft.Extensions.Configuration;

namespace ClientImport.Services
{
    public class PostService : IPostService
    {
        private readonly string _postApiRequestTemplate;

        public PostService(IConfiguration configuration)
        {
            var postApiUrl = configuration.GetValue<string>("PostApiUrl");
            var postApiKey = configuration.GetValue<string>("PostApiKey");
            _postApiRequestTemplate = $"{postApiUrl}/?term={{0}}&key={postApiKey}";
        }

        /// <summary>
        /// Looks for post index for provided address.
        /// </summary>
        /// <param name="address">Client address</param>
        /// <returns>Post index</returns>
        public async Task<string> GetPostIndexAsync(string address)
        {
            using (var httpClient = new HttpClient())
            {
                string requestUri = string.Format(_postApiRequestTemplate, address.Replace(" ","+"));
                var serializedResponse = await httpClient.GetAsync(requestUri);
                if (serializedResponse.IsSuccessStatusCode)
                {
                    var response = JsonConvert.DeserializeObject<PostResponseDTO>(await serializedResponse.Content.ReadAsStringAsync());
                    if (response.status == "success" && response.data.Length > 0)
                        return response.data[0].post_code;
                }
            }
            return null;
        }
    }
}
