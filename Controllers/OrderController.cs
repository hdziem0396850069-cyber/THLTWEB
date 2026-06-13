using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using Webbanhang_TH02.Models;
using Webbanhang_TH02.Repositories;

namespace Webbanhang_TH02.Controllers
{
    public class OrderController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IOrderRepository _orderRepository;
        private const string CART_KEY = "Shopping_Cart";

        public OrderController(IProductRepository productRepository, ICategoryRepository categoryRepository, IOrderRepository orderRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _orderRepository = orderRepository;
        }

        // GET: Order/Checkout?productId=1&quantity=1
        public IActionResult Checkout(int? productId, int quantity = 1)
        {
            var order = new Order();

            // Load promo code from session if any
            var promoCode = HttpContext.Session.GetString("PromoCode");
            var discountStr = HttpContext.Session.GetString("DiscountAmount");
            decimal discount = 0;
            if (decimal.TryParse(discountStr, out decimal parsedDiscount))
            {
                discount = parsedDiscount;
            }

            ViewBag.PromoCode = promoCode;
            ViewBag.DiscountAmount = discount;

            if (productId.HasValue && productId.Value > 0)
            {
                var product = _productRepository.GetById(productId.Value);
                if (product == null) return NotFound();

                var categories = _categoryRepository.GetAllCategories();
                ViewBag.CategoryName = categories.FirstOrDefault(c => c.Id == product.CategoryId)?.Name ?? "Mỹ phẩm";
                ViewBag.Product = product;

                order.ProductId = productId.Value;
                order.Quantity = quantity > 0 ? quantity : 1;
                order.TotalAmount = (product.Price * order.Quantity) - discount;
                if (order.TotalAmount < 0) order.TotalAmount = 0;
            }
            else
            {
                // Checkout the shopping cart
                var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CART_KEY);
                if (cart == null || !cart.Any())
                {
                    TempData["CartError"] = "Giỏ hàng của bạn đang trống.";
                    return RedirectToAction("Index", "Cart");
                }

                ViewBag.CartItems = cart;
                
                // Set total amount for model (subtracted discount)
                decimal subtotal = cart.Sum(item => item.Product.Price * item.Quantity);
                order.TotalAmount = subtotal - discount;
                if (order.TotalAmount < 0) order.TotalAmount = 0;
            }

            // Prefill customer details if logged in
            if (User.Identity?.IsAuthenticated == true)
            {
                order.CustomerName = User.Identity.Name ?? "";
                order.Email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            }

            return View(order);
        }

        // POST: Order/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(Order order)
        {
            // Load promo code from session if any
            var promoCode = HttpContext.Session.GetString("PromoCode");
            var discountStr = HttpContext.Session.GetString("DiscountAmount");
            decimal discount = 0;
            if (decimal.TryParse(discountStr, out decimal parsedDiscount))
            {
                discount = parsedDiscount;
            }

            if (ModelState.IsValid)
            {
                order.OrderDate = DateTime.Now;
                order.OrderCode = "HD" + DateTime.Now.ToString("yyyyMMddHHmmss");
                order.PromoCode = promoCode;
                order.DiscountAmount = discount;

                // Check if single product or cart checkout
                if (order.ProductId > 0)
                {
                    var product = _productRepository.GetById(order.ProductId);
                    if (product == null) return NotFound();

                    order.TotalAmount = (product.Price * order.Quantity) - discount;
                    if (order.TotalAmount < 0) order.TotalAmount = 0;

                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        ProductImageUrl = product.ImageUrl,
                        Price = product.Price,
                        Quantity = order.Quantity
                    });
                }
                else
                {
                    // Cart Checkout
                    var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CART_KEY);
                    if (cart == null || !cart.Any())
                    {
                        ModelState.AddModelError(string.Empty, "Giỏ hàng trống.");
                        return View(order);
                    }

                    foreach (var item in cart)
                    {
                        order.OrderItems.Add(new OrderItem
                        {
                            ProductId = item.Product.Id,
                            ProductName = item.Product.Name,
                            ProductImageUrl = item.Product.ImageUrl,
                            Price = item.Product.Price,
                            Quantity = item.Quantity
                        });
                    }

                    decimal subtotal = cart.Sum(item => item.Product.Price * item.Quantity);
                    order.TotalAmount = subtotal - discount;
                    if (order.TotalAmount < 0) order.TotalAmount = 0;

                    // Clear Cart & Promo from session
                    HttpContext.Session.Remove(CART_KEY);
                    HttpContext.Session.Remove("PromoCode");
                    HttpContext.Session.Remove("DiscountAmount");
                }

                // Save Order
                _orderRepository.Add(order);

                TempData["OrderCode"] = order.OrderCode;
                return RedirectToAction(nameof(Success));
            }

            // If not valid, rebuild views
            ViewBag.PromoCode = promoCode;
            ViewBag.DiscountAmount = discount;

            if (order.ProductId > 0)
            {
                var product = _productRepository.GetById(order.ProductId);
                ViewBag.Product = product;
                if (product != null)
                {
                    var categories = _categoryRepository.GetAllCategories();
                    ViewBag.CategoryName = categories.FirstOrDefault(c => c.Id == product.CategoryId)?.Name ?? "Mỹ phẩm";
                }
            }
            else
            {
                var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CART_KEY);
                ViewBag.CartItems = cart;
            }

            return View(order);
        }

        // GET: Order/Success
        public IActionResult Success()
        {
            var code = TempData["OrderCode"] as string;
            if (string.IsNullOrEmpty(code))
                return RedirectToAction("Index", "Product");

            var order = _orderRepository.GetByCode(code);
            if (order == null)
                return RedirectToAction("Index", "Product");

            ViewBag.OrderCode = order.OrderCode;
            ViewBag.CustomerName = order.CustomerName;
            ViewBag.Phone = order.Phone;
            ViewBag.Address = order.Address;
            ViewBag.Note = order.Note;
            ViewBag.PaymentMethod = order.PaymentMethod;
            ViewBag.OrderDate = order.OrderDate.ToString("dd/MM/yyyy HH:mm");
            ViewBag.TotalAmount = order.TotalAmount;
            ViewBag.PromoCode = order.PromoCode;
            ViewBag.DiscountAmount = order.DiscountAmount;

            if (order.OrderItems.Count == 1)
            {
                var firstItem = order.OrderItems.First();
                var product = _productRepository.GetById(firstItem.ProductId);
                ViewBag.Product = product;
                ViewBag.Quantity = firstItem.Quantity;
            }

            return View(order);
        }

        // GET: Order/List
        [Authorize(Roles = "Admin")]
        public IActionResult List()
        {
            var orders = _orderRepository.GetAll();
            return View(orders);
        }

        // GET: Order/MyOrders
        [Authorize]
        public IActionResult MyOrders()
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "";
            var orders = _orderRepository.GetAll()
                .Where(o => o.Email != null && o.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Calculate membership points & tier based on total spent
            // 10,000 VND spent = 1 point
            decimal totalSpent = orders.Sum(o => o.TotalAmount);
            int points = (int)(totalSpent / 10000);

            string tier = "Thành viên Đồng (Bronze)";
            int nextTierPoints = 100;
            string nextTierName = "Thành viên Bạc (Silver)";
            int progress = 0;

            if (points >= 600)
            {
                tier = "Thành viên Kim cương (Platinum)";
                nextTierPoints = 600;
                nextTierName = "Cấp độ cao nhất";
                progress = 100;
            }
            else if (points >= 300)
            {
                tier = "Thành viên Vàng (Gold)";
                nextTierPoints = 600;
                nextTierName = "Thành viên Kim cương (Platinum)";
                progress = (int)((points - 300) / 300.0 * 100);
            }
            else if (points >= 100)
            {
                tier = "Thành viên Bạc (Silver)";
                nextTierPoints = 300;
                nextTierName = "Thành viên Vàng (Gold)";
                progress = (int)((points - 100) / 200.0 * 100);
            }
            else
            {
                tier = "Thành viên Đồng (Bronze)";
                nextTierPoints = 100;
                nextTierName = "Thành viên Bạc (Silver)";
                progress = (int)(points / 100.0 * 100);
            }

            ViewBag.Points = points;
            ViewBag.Tier = tier;
            ViewBag.NextTierPoints = nextTierPoints;
            ViewBag.NextTierName = nextTierName;
            ViewBag.Progress = progress;
            ViewBag.TotalSpent = totalSpent;

            return View(orders);
        }
    }
}
