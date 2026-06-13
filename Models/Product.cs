using System.ComponentModel.DataAnnotations;

namespace Webbanhang_TH02.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [StringLength(100, ErrorMessage = "Tên sản phẩm không quá 100 ký tự")]
        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập giá sản phẩm")]
        [Range(1000, 100000000, ErrorMessage = "Giá sản phẩm phải nằm trong khoảng từ 1,000đ đến 100,000,000đ")]
        [Display(Name = "Giá sản phẩm (VNĐ)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mô tả")]
        [Display(Name = "Mô tả sản phẩm")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

        [Display(Name = "Đường dẫn ảnh")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Thương hiệu")]
        public string? Brand { get; set; }

        [Range(1, 5)]
        [Display(Name = "Đánh giá")]
        public int Rating { get; set; } = 5;

        [Display(Name = "Sản phẩm nổi bật")]
        public bool IsFeatured { get; set; } = false;
    }
}
