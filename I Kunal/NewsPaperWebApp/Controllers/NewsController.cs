using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsPaperWebApp.Services;
using System.Threading.Tasks;

namespace NewsPaperWebApp.Controllers
{
    [Authorize] // Only logged-in users can access news
    public class NewsController : Controller
    {
        private readonly NewsService _newsService;

        public NewsController(NewsService newsService)
        {
            _newsService = newsService;
        }

        public async Task<IActionResult> Index()
        {
            var news = await _newsService.GetLatestNews();
            return View(news);
        }
    }
}
