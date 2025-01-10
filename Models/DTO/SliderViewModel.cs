using System.ComponentModel.DataAnnotations;

namespace SchoolProj.Models.DTO
{
    public class SliderViewModel
    {

        [Required(ErrorMessage ="Please select slider image")]
        public IFormFile SliderImage { get; set; }

        [Required(ErrorMessage = "Please enter slider title")]
        public string SLiderTitle { get; set; }

        [Required(ErrorMessage = "Please enter slider description")]
        public string SliderDesc { get; set; }

    }
}
