using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P335_BackEnd.Data;
using P335_BackEnd.Models;
using P335_BackEnd.Services;
using System.Diagnostics;

namespace P335_BackEnd.Controllers
{
	public class HomeController : Controller
	{
        private readonly AppDbContext _dbContext;
        private readonly ProductService _productService;

        public HomeController(AppDbContext dbContext, ProductService productService)
        {
            _dbContext = dbContext;
            _productService = productService;
        }

        public IActionResult Index(int? categoryId)
		{
            var categories = _dbContext.Categories.Include(x=>x.CategoryImage).ToList();
            if (categories is null) return NotFound();

            var featured = _productService.GetFeaturedProducts(categoryId);

            if (featured is null) return NotFound();

            var latest = _productService.GetLatestProducts();

            if (latest is null) return NotFound();

            var topRated = _productService.GetTopRatedProducts();

            if (topRated is null) return NotFound();

            var review = _productService.GetReviewProducts();

            if (review is null) return NotFound();

            var model = new HomeIndexVM
            {
                Categories = categories,
                FeaturedProducts = featured,
                LatestProducts = latest,
                TopRatedProducts = topRated,
                ReviewProducts = review
            };
			return View(model);
		}
	}
}
