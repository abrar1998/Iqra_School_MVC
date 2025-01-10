using SchoolProj.Models.Domain;
using SchoolProj.Models.DTO;

namespace SchoolProj.DAL.PageLoadService
{
    public interface IPageLoadRepo
    {
        Task LoadAllPagesOnceAsync();
        Task<Page?> GetPageByIdAsync(string pageId);
        Task LoadAllSlidesOnceAsync();
        List<Slider> GetSlidersAsync();
        Task GetNoticeListWithFilesAsync();
        List<NoticeDTO> GetNotices();
    }
}
