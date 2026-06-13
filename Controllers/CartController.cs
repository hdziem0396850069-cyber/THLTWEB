using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Webbanhang_TH02.Models;
using Webbanhang_TH02.Repositories;

namespace Webbanhang_TH02.Controllers
{
    public class CartController : Controller
    {
        private const string CART_KEY = "Shopping_Cart";
        private readonly IProductRepository _productRepository;

        public CartController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // GET: Cart
        public IActionResult Index()
        {
            var cart = GetCartItems();
            return View(cart);
        }

        // POST: Cart/AddToCart
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            var product = _productRepository.GetById(productId);
            if (product == null)
            {
                return NotFound();
            }

            var cart = GetCartItems();
            var cartItem = cart.FirstOrDefault(item => item.Product.Id == productId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem { Product = product, Quantity = quantity });
            }

            SaveCartItems(cart);

            // Redirect back to referring page or to Cart page
            return RedirectToAction(nameof(Index));
        }

        // POST: Cart/UpdateQuantity
        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            if (quantity < 1)
            {
                return RemoveFromCart(productId);
            }

            var cart = GetCartItems();
            var cartItem = cart.FirstOrDefault(item => item.Product.Id == productId);

            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                SaveCartItems(cart);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET/POST: Cart/RemoveFromCart
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCartItems();
            var cartItem = cart.FirstOrDefault(item => item.Product.Id == productId);

            if (cartItem != null)
            {
                cart.Remove(cartItem);
                SaveCartItems(cart);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Cart/ClearCart
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove(CART_KEY);
            return RedirectToAction(nameof(Index));
        }

        private List<CartItem> GetCartItems()
        {
            return HttpContext.Session.GetObjectFromJson<List<CartItem>>(CART_KEY) ?? new List<CartItem>();
        }

        private void SaveCartItems(List<CartItem> cart)
        {
            HttpContext.Session.SetObjectAsJson(CART_KEY, cart);
            RecalculateDiscount(cart);
        }

        private void RecalculateDiscount(List<CartItem> cart)
        {
            var promoCode = HttpContext.Session.GetString("PromoCode");
            if (string.IsNullOrEmpty(promoCode) || !cart.Any())
            {
                HttpContext.Session.Remove("PromoCode");
                HttpContext.Session.Remove("DiscountAmount");
                return;
            }

            decimal total = cart.Sum(item => item.Product.Price * item.Quantity);
            decimal discount = 0;

            if (promoCode == "HONGDIEM20")
            {
                discount = total * 0.20m;
            }
            else if (promoCode == "KHOIMOI10")
            {
                discount = total * 0.10m;
            }
            else if (promoCode == "GIAM50K")
            {
                if (total >= 300000)
                {
                    discount = 50000;
                }
                else
                {
                    HttpContext.Session.Remove("PromoCode");
                    HttpContext.Session.Remove("DiscountAmount");
                    return;
                }
            }

            HttpContext.Session.SetString("DiscountAmount", discount.ToString());
        }

        // POST: Cart/ApplyPromoCode
        [HttpPost]
        public IActionResult ApplyPromoCode(string promoCode)
        {
            if (string.IsNullOrEmpty(promoCode))
            {
                TempData["PromoError"] = "Vui lòng nhập mã khuyến mãi.";
                return RedirectToAction(nameof(Index));
            }

            var cart = GetCartItems();
            if (!cart.Any())
            {
                TempData["PromoError"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction(nameof(Index));
            }

            decimal totalBeforeDiscount = cart.Sum(item => item.Product.Price * item.Quantity);
            decimal discountAmount = 0;
            string codeLower = promoCode.Trim().ToUpper();

            if (codeLower == "HONGDIEM20")
            {
                discountAmount = totalBeforeDiscount * 0.20m;
            }
            else if (codeLower == "KHOIMOI10")
            {
                discountAmount = totalBeforeDiscount * 0.10m;
            }
            else if (codeLower == "GIAM50K")
            {
                if (totalBeforeDiscount >= 300000)
                {
                    discountAmount = 50000;
                }
                else
                {
                    TempData["PromoError"] = "Mã GIAM50K chỉ áp dụng cho đơn hàng từ 300,000đ trở lên.";
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                TempData["PromoError"] = "Mã khuyến mãi không hợp lệ hoặc đã hết hạn.";
                return RedirectToAction(nameof(Index));
            }

            HttpContext.Session.SetString("PromoCode", codeLower);
            HttpContext.Session.SetString("DiscountAmount", discountAmount.ToString());
            TempData["PromoSuccess"] = $"Áp dụng mã {codeLower} thành công!";
            
            return RedirectToAction(nameof(Index));
        }

        // GET/POST: Cart/RemovePromoCode
        public IActionResult RemovePromoCode()
        {
            HttpContext.Session.Remove("PromoCode");
            HttpContext.Session.Remove("DiscountAmount");
            TempData["PromoSuccess"] = "Đã hủy áp dụng mã khuyến mãi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
