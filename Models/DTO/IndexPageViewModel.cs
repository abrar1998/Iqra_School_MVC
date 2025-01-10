using SchoolProj.Models.Domain;

namespace SchoolProj.Models.DTO
{
    public class IndexPageViewModel
    {
        public List<Slider>? Sliders { get; set; } = new List<Slider>();
        public List<Photos>? Categories { get; set; } = new List<Photos> ();
        public List<NoticeDTO>? LatestNews { get; set; } = new List<NoticeDTO>();
        public List<YoutubeDTO>? Youtube { get; set; }  = new List <YoutubeDTO> ();
        public Page? DirectorMessage { get; set; } = new Page();
        
    }
}
