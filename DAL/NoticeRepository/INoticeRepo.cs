

using SchoolProj.Models;
using SchoolProj.Models.Domain;
using SchoolProj.Models.DTO;

namespace SchoolProj.DAL.NoticeRepository
{
    public interface INoticeRepo
    {
        Task<ResponseDTO<List<NoticeDTO>>> GetNoticeListAsync(); //this will give use notice list
        Task<ResponseDTO<Notice>> CreateNoticeAsync(Notice notice);
        Task<ResponseDTO<List<Notice>>> GetCategoryListAsync(); //this method will return us category list

        Task<ResponseDTO<NoticeDTO>> GetNoticeByIdAsync(string nId);
        Task<ResponseDTO<NoticeDTO>> DeleteNoticeByIdAsync(NoticeDTO currentNotice,string nId);
        Task<ResponseDTO<List<NoticeDTO>>> GetNoticeListWithFilesAsync();

    }
}
