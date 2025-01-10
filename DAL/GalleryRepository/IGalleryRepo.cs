using SchoolProj.Models.Domain;
using SchoolProj.Models;

namespace SchoolProj.DAL.GalleryRepository
{
    public interface IGalleryRepo
    {
        Task<ResponseDTO<Photos>> CreatePhotoCategoryAsync(Photos photo);
        Task<ResponseDTO<List<Photos>>> GetPhotoCategoriesAsync();
        Task<ResponseDTO<Photos>> DeletePhotoCategoryAsync(Photos photo);
        List<Photos> GeneratePhotosUrl(List<Photos> _photos);
        Task<ResponseDTO<Photos>> UpdatePhotoCategoryAsync(Photos photo);
        Task<ResponseDTO<Photos>> CreatePhotoAsync(Photos photo);
        Task<ResponseDTO<Photos>> DeletePhotoAsync(Photos photo);
        Task<ResponseDTO<List<Photos>>> GetImagesByCategoryIdAsync(string categoryId);
        Task<ResponseDTO<List<Photos>>> GetPhotoIdAsync(string pid);
        //youtube section
        Task<ResponseDTO<Photos>> AddYoutubeLInkAsync(Photos photo);
        Task<ResponseDTO<List<Photos>>> GetAllYoutubeLinksAsync(string categoryId);
    }
}
