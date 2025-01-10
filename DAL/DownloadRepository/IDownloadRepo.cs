using SchoolProj.Models.Domain;
using SchoolProj.Models;
using Newtonsoft.Json;
using SchoolProj.General;
using System.Net.Http;

namespace SchoolProj.DAL.DownloadRepository
{
    public interface IDownloadRepo
    {
        Task<ResponseDTO<Downloads>> CreateDownloadsAsync(Downloads download);
        Task<ResponseDTO<List<Downloads>>> GetDownloadForStudent(string dId, string dType);
        Task<ResponseDTO<List<Downloads>>> GetDownloadAsync(string dId);
        Task<ResponseDTO<List<Downloads>>> GetAllDownloadsAsync();
        Task<ResponseDTO<Downloads>> DeleteDownloadAsync(Downloads download);
        Task<ResponseDTO<List<Downloads>>> GetAllDownloadsTypesForDropDownAsync();
        Task<ResponseDTO<List<Downloads>>> GetAllClassesForDropDownAsync();


    }
}
