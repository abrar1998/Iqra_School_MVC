using System.ComponentModel.DataAnnotations;

namespace SchoolProj.Models.DTO
{
    public class PageViewModel
    {

        [Required(ErrorMessage ="Plese Enter Page Name")]
        public string PageName { get; set; }

        [Required(ErrorMessage = "Plese Enter Page Title")]
        public string PageTitle { get; set; }

        [Required(ErrorMessage = "Plese Select Page Photo")]
        public IFormFile PagePic { get; set; }

        [Required(ErrorMessage = "Plese Enter Page url")]
        public string PageUrl { get; set; }

        [Required(ErrorMessage = "Plese Enter Page Heading")]
        public string PageHeading { get; set; }

        [Required(ErrorMessage = "Plese Enter Page Data")]
        public string PageData { get; set; }
        public string? Download { get; set; }
        public string PageCat { get; set; } = "1";
        public string? ThumbNail { get; set; }
    }
}
