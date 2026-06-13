using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Webbanhang_TH02.Models;
using Webbanhang_TH02.Repositories;

namespace Webbanhang_TH02.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        // Display a list of products
        public IActionResult Index(int? categoryId)
        {
            var products = _productRepository.GetAll();
            var categories = _categoryRepository.GetAllCategories();

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
                ViewBag.CurrentCategoryId = categoryId.Value;
            }

            ViewBag.Categories = categories;
            return View(products);
        }

        // Display a single product detail
        public IActionResult Display(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            
            var categories = _categoryRepository.GetAllCategories();
            var categoryName = categories.FirstOrDefault(c => c.Id == product.CategoryId)?.Name ?? "Không xác định";
            ViewBag.CategoryName = categoryName;

            return View(product);
        }

        // Show the product add form
        [Authorize(Roles = "Admin")]
        public IActionResult Add()
        {
            var categories = _categoryRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        // Process the product addition
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Add(Product product)
        {
            if (ModelState.IsValid)
            {
                _productRepository.Add(product);
                return RedirectToAction(nameof(Index));
            }
            
            var categories = _categoryRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // Show the product update form
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            var categories = _categoryRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // Process the product update
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(Product product)
        {
            if (ModelState.IsValid)
            {
                _productRepository.Update(product);
                return RedirectToAction(nameof(Index));
            }

            var categories = _categoryRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // Show the product delete confirmation page
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            var categories = _categoryRepository.GetAllCategories();
            ViewBag.CategoryName = categories.FirstOrDefault(c => c.Id == product.CategoryId)?.Name ?? "Không xác định";
            return View(product);
        }

        // Process the product deletion
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            _productRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        // Admin Product Management List
        [Authorize(Roles = "Admin")]
        public IActionResult Management()
        {
            var products = _productRepository.GetAll();
            var categories = _categoryRepository.GetAllCategories();
            ViewBag.Categories = categories;
            return View(products);
        }

        // Hàm hiển thị giao diện mới khi bấm Mua ngay
        public IActionResult Checkout(int id)
        {
            // Tìm sản phẩm dựa vào ID truyền tới
            var product = _productRepository.GetById(id);

            if (product == null)
            {
                return NotFound();
            }

            // Trả về file Checkout.cshtml kèm theo thông tin sản phẩm
            return View(product);
        }
    }
}
