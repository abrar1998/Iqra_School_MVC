using System.ComponentModel.DataAnnotations;

namespace SchoolProj.Models.DTO
{
    public class YoutubeDTO
    {
        public string? Yid {  get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Video Link is required")]
        public string Url { get; set; }
        public string? Date { get; set; }
        public string? VideoId { get; set; }

    }
}
