using System.ComponentModel.DataAnnotations;

namespace SchoolProj.Models.DTO
{
    public class NoticeViewModel
    {
        public string? Nid { get; set; }

        [Required(ErrorMessage ="Enter Notification Date")]
        public string NDate { get; set; }

        [Required(ErrorMessage = "Enter Notification Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Enter Notification Description")]
        public string Description { get; set; }
        public IFormFile? FilePath { get; set; }
        public string? Url { get; set; }

        [Required(ErrorMessage = "Please Select Notification Type")]
        public string Ncid { get; set; }
        public string? NcName { get; set; }
        public string? UserName { get; set; }
        public IFormFile? ThumbNail { get; set; }
        public int? IsFile { get; set; }
        public int? ActionType { get; set; }
    }
}
