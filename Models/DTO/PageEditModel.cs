using System.ComponentModel.DataAnnotations;

namespace SchoolProj.Models.DTO
{
    public class PageEditModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Please Enter Page Name")]
        public string PageName { get; set; }

        [Required(ErrorMessage = "Please Enter Page Title")]
        public string PageTitle { get; set; }

        public string ExistingPagePic { get; set; } // Holds the path of the current picture

        public IFormFile? NewPagePic { get; set; } // Allows uploading a new picture

        [Required(ErrorMessage = "Please Enter Page URL")]
        public string PageUrl { get; set; }

        [Required(ErrorMessage = "Please Enter Page Heading")]
        public string PageHeading { get; set; }

        [Required(ErrorMessage = "Please Enter Page Data")]
        public string PageData { get; set; }
    }
}
