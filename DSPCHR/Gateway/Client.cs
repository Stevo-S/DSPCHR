using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DSPCHR.Gateway
{
    public class Client
    {
        private readonly HttpClient _client;
        public Client(HttpClient httpClient, IConfiguration configuration)
        {
            httpClient.BaseAddress = new Uri(configuration["Gateway:Address"]);
            _client = httpClient;
        }

        // Make HTTP request to the gateway
        public async Task Send(string urlPath, string body)
        {
            var url = new Uri(_client.BaseAddress + urlPath);
            var requestBody = new StringContent(body);

            // Log Web Request
            //var webRequest = new WebRequest();
            //webRequest.Body = body;
            //webRequest.TimeStamp = DateTime.Now;
            //_databaseContext.Add(webRequest);


            requestBody.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
            //requestBody.Headers.Add("X-Authorization", "Bearer " + _tokenManager.AccessToken);
            //requestBody.Headers.Add("sourceAddress", "1xx.2xx.2x7.xxx");
            //webRequest.Body = webRequest.Body + requestBody.Headers.ToString();

            try
            {
                var response = _client.PostAsync(url, requestBody).Result;
                _ = await response.Content.ReadAsStringAsync();
            }
            catch(Exception e)
            {
                // TODO: Handle specific exceptions
                ;
            }
            //var responseBody = await response.Content.ReadAsStringAsync();

            // Log Web Response
            //var webResponse = new WebResponse();
            //webResponse.Body = responseBody;
            //webResponse.TimeStamp = DateTime.Now;
            //_databaseContext.Add(webResponse);

            //_databaseContext.SaveChanges();
        }
    }
}
