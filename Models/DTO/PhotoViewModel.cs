using System.ComponentModel.DataAnnotations;

namespace SchoolProj.Models.DTO
{
    public class PhotoViewModel
    {
        [Required(ErrorMessage ="image is required")]
        public IFormFile Image { get; set; }
        
        public string? Title { get; set; }
        public string? Description { get; set; }

        [Required(ErrorMessage ="date is required")]
        public string PhotoDate { get; set; }
        [Required(ErrorMessage = "album type is required")]
        public string PCIDFK { get; set; }
    }
}
