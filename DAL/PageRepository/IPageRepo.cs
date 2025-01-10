using SchoolProj.Models;
using SchoolProj.Models.Domain;
using SchoolProj.Models.DTO;

namespace SchoolProj.DAL.PageRepository
{
    public interface IPageRepo
    {
        Task<ResponseDTO<Page>> CreatePageAsync(Page page);
        Task<ResponseDTO<List<Page>>> GetAllPagesAsync();
        Task<ResponseDTO<List<Page>>> GetPageAsync(string id);
        PageEditModel GetPageEditViewModel(Page page);
        Task<ResponseDTO<Page>> UpdatePageAsync(Page page);
    }
}
