using System.ComponentModel.DataAnnotations;

namespace SchoolProj.Models.DTO
{
    public class CategoryViewEditModel
    {
        [Required(ErrorMessage = "Category Id Missing")]
        public string CategoryId { get; set; }
        [Required(ErrorMessage = "Category name can't be empty")]
        public string CategoryName { get; set; }
        [Required(ErrorMessage = "category date cant be empty")]
        public string CategoryDate { get; set; }
        public IFormFile? Thumbnail { get; set; }
    }
}
