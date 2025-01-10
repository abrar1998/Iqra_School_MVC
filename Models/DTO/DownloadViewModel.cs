using System.ComponentModel.DataAnnotations;

namespace SchoolProj.Models.DTO
{
    public class DownloadViewModel
    {

        [Required(ErrorMessage ="please enter download title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "please enter download description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please select file")]
        public IFormFile FilePath { get; set; }

        [Required(ErrorMessage ="Please select download type")]
        public string DType { get; set; }

        [Required(ErrorMessage ="please select any class")]
        public string ClassID { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime DDate { get; set; }
    }
}
