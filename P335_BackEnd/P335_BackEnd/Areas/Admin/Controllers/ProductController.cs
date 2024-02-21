using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P335_BackEnd.Areas.Admin.Models;
using P335_BackEnd.Data;
using P335_BackEnd.Entities;
using P335_BackEnd.Migrations;
using P335_BackEnd.Services;

namespace P335_BackEnd.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly FileService _fileService;

        public ProductController(AppDbContext dbContext, FileService fileService)
        {
            _dbContext = dbContext;
            _fileService = fileService;
        }

        public IActionResult Index()
        {
            var products = _dbContext.Products.AsNoTracking().ToList();

            var model = new ProductIndexVM
            {
                Products = products
            };

            return View(model);
        }

        public IActionResult Add()
        {
            var categories = _dbContext.Categories.AsNoTracking().ToList();
            var productTypes = _dbContext.ProductTypes.AsNoTracking().ToList();

            var model = new ProductAddVM
            {
                Categories = categories,
                ProductTypes = productTypes
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Add(ProductAddVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var newProduct = new Product();

            newProduct.Name = model.Name;
            newProduct.Price = (decimal)model.Price;

            var foundCategory = _dbContext.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
            if (foundCategory is null) return View(model);

            newProduct.Category = foundCategory;

            if (model.ProductTypeId != null)
            {
                var foundProductType = _dbContext.ProductTypes.FirstOrDefault(x => x.Id == model.ProductTypeId);
                if (foundProductType is null) return View(model);

                newProduct.ProductTypeProducts = new()
                {
                    new ProductTypeProduct
                    {
                        ProductType = foundProductType
                    }
                };
            }

            newProduct.ImageUrl = _fileService.AddFile(model.Image, Path.Combine("img", "featured"));

            _dbContext.Add(newProduct);
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update(int id)
        {
            var product = _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.ProductTypeProducts)
                    .ThenInclude(ptp => ptp.ProductType)
                .FirstOrDefault(x => x.Id == id);

            if (product is null)
            {
                return NotFound();
            }

            var categories = _dbContext.Categories.AsNoTracking().ToList();
            var productTypes = _dbContext.ProductTypes.AsNoTracking().ToList();

            var model = new ProductEditVM
            {
                Name = product.Name,
                Price = product.Price,
                CategoryId = product.Category?.Id ?? 0,
                Categories = categories,
                ProductTypeId = product.ProductTypeProducts?.FirstOrDefault()?.ProductType?.Id,
                ProductTypes = productTypes
            };

            return View("Update", model);
        }

        [HttpPost]
        public IActionResult Update(int id, ProductEditVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingProduct = _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.ProductTypeProducts)
                    .ThenInclude(ptp => ptp.ProductType)
                .FirstOrDefault(x => x.Id == id);

            if (existingProduct is null)
            {
                return NotFound();
            }


            _fileService.DeleteFile(existingProduct.ImageUrl, Path.Combine("img", "featured"));

            existingProduct.Name = model.Name;
            existingProduct.Price = (decimal)model.Price;

            var foundCategory = _dbContext.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
            if (foundCategory is null) return View(model);

            existingProduct.Category = foundCategory;

            if (model.ProductTypeId != null)
            {
                var foundProductType = _dbContext.ProductTypes.FirstOrDefault(x => x.Id == model.ProductTypeId);
                if (foundProductType is null) return View(model);

                existingProduct.ProductTypeProducts = new List<ProductTypeProduct>
            {
                new ProductTypeProduct
                {
                    ProductType = foundProductType
                }
            };
            }

            existingProduct.ImageUrl = _fileService.AddFile(model.Image, Path.Combine("img", "featured"));

            _dbContext.Products.Update(existingProduct);
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var product = _dbContext.Products.FirstOrDefault(x => x.Id == id);

            if (product is null) return NotFound();

            _fileService.DeleteFile(product.ImageUrl, Path.Combine("img", "featured"));

            _dbContext.Remove(product);

            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
