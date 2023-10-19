using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace COSHH_Generator.Scrapers
{
    static class Client
    {
        private static HttpClient httpClient = new HttpClient(new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.All
        });
        public static Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return httpClient.SendAsync(request);
        }
        
        public static Task<HttpResponseMessage> GetAsync(string url)
        {
            return httpClient.GetAsync(url);
        }
    }
}
