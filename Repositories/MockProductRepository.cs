using System.Collections.Generic;
using System.Linq;
using Webbanhang_TH02.Models;

namespace Webbanhang_TH02.Repositories
{
    public class MockProductRepository : IProductRepository
    {
        private readonly List<Product> _products;

        public MockProductRepository()
        {
            _products = new List<Product>
            {
                new Product 
                { 
                    Id = 1, 
                    Name = "Serum Phục Hồi Da B5 & HA", 
                    Price = 350000,
                    Description = "Serum phục hồi da chứa Vitamin B5 và Hyaluronic Acid cao cấp, giúp cấp ẩm sâu, làm dịu da nhạy cảm, phục hồi hàng rào bảo vệ da khỏe mạnh.",
                    CategoryId = 1,
                    ImageUrl = "/images/skincare_serum.jpg",
                    Brand = "Hồng Diễm",
                    Rating = 5,
                    IsFeatured = true
                },
                new Product 
                { 
                    Id = 2, 
                    Name = "Son Kem Lì Velvet Matte", 
                    Price = 280000,
                    Description = "Son kem lì siêu mịn môi, chất son nhẹ tênh như nhung, màu đỏ đất thời thượng, bền màu suốt 8 tiếng không khô môi.",
                    CategoryId = 2,
                    ImageUrl = "/images/lipstick_matte.jpg",
                    Brand = "Hồng Diễm",
                    Rating = 5,
                    IsFeatured = true
                },
                new Product 
                { 
                    Id = 3, 
                    Name = "Bảng Phấn Mắt Nude Glam", 
                    Price = 420000,
                    Description = "Bảng phấn mắt 9 màu nhũ và lì tông ấm cực kỳ thời thượng, độ bám màu cao, dễ dàng phối hợp cho trang điểm đi làm hoặc đi tiệc.",
                    CategoryId = 3,
                    ImageUrl = "/images/eyeshadow_palette.jpg",
                    Brand = "Hồng Diễm",
                    Rating = 4,
                    IsFeatured = true
                },
                new Product 
                { 
                    Id = 4, 
                    Name = "Nước Hoa Nữ Golden Hour EDP", 
                    Price = 950000,
                    Description = "Nước hoa mang hương thơm ngọt ngào, quyến rũ từ các loại hoa phương Đông kết hợp với nốt hương ấm áp của gỗ tuyết tùng và hổ phách.",
                    CategoryId = 4,
                    ImageUrl = "/images/perfume_gold.jpg",
                    Brand = "Hồng Diễm",
                    Rating = 5,
                    IsFeatured = true
                },
                new Product 
                { 
                    Id = 5, 
                    Name = "Kem Dưỡng Khóa Ẩm Aqua Cream", 
                    Price = 390000,
                    Description = "Gel dưỡng ẩm mỏng nhẹ, thấm nhanh tức thì, khóa ẩm sâu 24 giờ mang lại làn da căng bóng ngậm nước rạng rỡ.",
                    CategoryId = 1,
                    ImageUrl = "/images/skincare_cream.jpg",
                    Brand = "Hồng Diễm",
                    Rating = 4,
                    IsFeatured = false
                }
            };
        }

        public IEnumerable<Product> GetAll()
        {
            return _products;
        }

        public Product? GetById(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

        public void Add(Product product)
        {
            product.Id = _products.Any() ? _products.Max(p => p.Id) + 1 : 1;
            if (string.IsNullOrEmpty(product.ImageUrl))
            {
                product.ImageUrl = "/images/placeholder_cosmetics.jpg";
            }
            if (string.IsNullOrEmpty(product.Brand))
            {
                product.Brand = "Hồng Diễm";
            }
            _products.Add(product);
        }

        public void Update(Product product)
        {
            var index = _products.FindIndex(p => p.Id == product.Id);
            if (index != -1)
            {
                // Preserve image if not updated
                if (string.IsNullOrEmpty(product.ImageUrl))
                {
                    product.ImageUrl = _products[index].ImageUrl;
                }
                if (string.IsNullOrEmpty(product.Brand))
                {
                    product.Brand = _products[index].Brand ?? "Hồng Diễm";
                }
                _products[index] = product;
            }
        }

        public void Delete(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _products.Remove(product);
            }
        }
    }
}
