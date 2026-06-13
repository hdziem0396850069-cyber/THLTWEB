using System.ComponentModel.DataAnnotations;

namespace Webbanhang_TH02.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [StringLength(50, ErrorMessage = "Tên danh mục không quá 50 ký tự")]
        public string Name { get; set; } = string.Empty;
    }
}
