using System.Text.Json;
using NewsPaperWebApp.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NewsPaperWebApp.Services
{
    public class NewsService
    {
        private readonly HttpClient _httpClient;

        public NewsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<NewsArticle>> GetLatestNews() 
        {
            try
            {
                var response = await _httpClient.GetStringAsync("http://localhost:5001/api/news");
                return JsonSerializer.Deserialize<List<NewsArticle>>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) 
                       ?? new List<NewsArticle>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error fetching news: {ex.Message}");
                return new List<NewsArticle>();
            }
        }
    }
}
