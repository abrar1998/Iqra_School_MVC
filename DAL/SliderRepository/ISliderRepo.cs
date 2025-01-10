using SchoolProj.Models.Domain;
using SchoolProj.Models;
using SchoolProj.Models.DTO;

namespace SchoolProj.DAL.SliderRepository
{
    public interface ISliderRepo
    {
        Task<ResponseDTO<Slider>> CreateSliderAsync(Slider slider);
        Task<ResponseDTO<List<Slider>>> GetSlidesAsync();
        Task<ResponseDTO<Slider>> DeleteSlideByIdAsync(string sId);
        Slider GetSlideByIdAsync(string sId);
        List<Slider> GetSlidesFromList();
        Task<ResponseDTO<List<NoticeDTO>>> GetNoticeListWithFilesAsync();
    }
}
