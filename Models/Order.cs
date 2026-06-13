using System.ComponentModel.DataAnnotations;

namespace Webbanhang_TH02.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        [Display(Name = "Số lượng")]
        [Range(1, 100, ErrorMessage = "Số lượng phải từ 1 đến 100")]
        public int Quantity { get; set; } = 1;

        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [Display(Name = "Họ và tên")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        [Display(Name = "Địa chỉ giao hàng")]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Ghi chú đơn hàng")]
        public string? Note { get; set; }

        [Display(Name = "Phương thức thanh toán")]
        public string PaymentMethod { get; set; } = "cod";

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Display(Name = "Mã đơn hàng")]
        public string OrderCode { get; set; } = string.Empty;

        [Display(Name = "Tổng tiền (VNĐ)")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Mã giảm giá")]
        public string? PromoCode { get; set; }

        [Display(Name = "Số tiền giảm (VNĐ)")]
        public decimal DiscountAmount { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new();
    }
}
