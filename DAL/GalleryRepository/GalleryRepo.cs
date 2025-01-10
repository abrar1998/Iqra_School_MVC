using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SchoolProj.General;
using SchoolProj.Models;
using SchoolProj.Models.Domain;
using SchoolProj.Models.DTO;
using System.Globalization;
using System.Text;

namespace SchoolProj.DAL.GalleryRepository
{
    public class GalleryRepo: IGalleryRepo
    {

        private readonly HttpClient _httpClient;
        private readonly IOptions<ApiSettings> _apiSettings;
        private readonly IHttpContextAccessor httpContextAccessor;

        public GalleryRepo(HttpClient httpClient, IOptions<ApiSettings> apiSettings, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(120); // Set timeout once during initialization
            _apiSettings = apiSettings;
            this.httpContextAccessor = httpContextAccessor;
        }

        //add new photo category PUT: api/photo/1
        public async Task<ResponseDTO<Photos>> CreatePhotoCategoryAsync(Photos photo)
        {
            try
            {
                string url = $"{_apiSettings.Value.BaseUrl}/Photo/1";
                var jsonNotice = JsonConvert.SerializeObject(photo);
                var content = new StringContent(jsonNotice, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PutAsync(url, content);
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var createdCatRes = JsonConvert.DeserializeObject<ResponseDTO<Photos>>(jsonResponse);
                return createdCatRes!;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred during the notice request: {ex.Message}", ex);
            }
        }

        //get all photo categories GET: api/photo
        public async Task<ResponseDTO<List<Photos>>> GetPhotoCategoriesAsync()
        {
            try
            {
                string url = $"{_apiSettings.Value.BaseUrl}/Photo";
                HttpResponseMessage response = await _httpClient.GetAsync(url);            
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var catRes = JsonConvert.DeserializeObject<ResponseDTO<List<Photos>>>(jsonResponse);
                if(catRes.ResponseData.Count()>0)
                {
                    catRes.ResponseData = catRes.ResponseData.Where(c => c.PCIDFK != "10014").ToList(); //because we i have created this category for youtuble links under this category id i will store youtube links, so we dont need to display this category
                }
                return catRes!;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred during the notice request: {ex.Message}", ex);
            }
        }

        public List<Photos> GeneratePhotosUrl(List<Photos> _photos)
        {
            foreach (var category in _photos)
            {
                category.ThumbNail = FileUploadHelper.GenerateFileUrl(category.ThumbNail, httpContextAccessor.HttpContext!.Request);
            }
            return _photos;
        }


        //delete photo category
        public async Task<ResponseDTO<Photos>> DeletePhotoCategoryAsync(Photos photo)
        {
            try
            {
                string url = $"{_apiSettings.Value.BaseUrl}/Photo/3";
                var jsonNotice = JsonConvert.SerializeObject(photo);
                var content = new StringContent(jsonNotice, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PutAsync(url, content);
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var deletedCatRes = JsonConvert.DeserializeObject<ResponseDTO<Photos>>(jsonResponse);
                return deletedCatRes!;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred during the notice request: {ex.Message}", ex);
            }
        }

        //update photo category
        public async Task<ResponseDTO<Photos>> UpdatePhotoCategoryAsync(Photos photo)
        {
            try
            {
                string url = $"{_apiSettings.Value.BaseUrl}/Photo/2";
                var jsonNotice = JsonConvert.SerializeObject(photo);
                var content = new StringContent(jsonNotice, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PutAsync(url, content);
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var updatedCatRes = JsonConvert.DeserializeObject<ResponseDTO<Photos>>(jsonResponse);
                return updatedCatRes!;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred during the notice request: {ex.Message}", ex);
            }
        }

        //create photo
        public async Task<ResponseDTO<Photos>> CreatePhotoAsync(Photos photo)
        {
            try
            {
                string url = $"{_apiSettings.Value.BaseUrl}/Photo";
                var jsonNotice = JsonConvert.SerializeObject(photo);
                var content = new StringContent(jsonNotice, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var createdPhotoRes = JsonConvert.DeserializeObject<ResponseDTO<Photos>>(jsonResponse);
                return createdPhotoRes!;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred during the notice request: {ex.Message}", ex);
            }
        }

        //delete photo
        public async Task<ResponseDTO<Photos>> DeletePhotoAsync(Photos photo)
        {
            try
            {
                DateTime parsedDate = DateTime.ParseExact(photo.PDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                photo.PDate = parsedDate.ToString("yyyy-MM-dd");
                string url = $"{_apiSettings.Value.BaseUrl}/Photo/4";
                var jsonNotice = JsonConvert.SerializeObject(photo);
                var content = new StringContent(jsonNotice, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PutAsync(url, content);
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var createdPhotoRes = JsonConvert.DeserializeObject<ResponseDTO<Photos>>(jsonResponse);
                return createdPhotoRes!;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred during the notice request: {ex.Message}", ex);
            }
        }

        //get images by category through cat id
        public async Task<ResponseDTO<List<Photos>>> GetImagesByCategoryIdAsync(string categoryId)
        {
            try
            {
                if (string.IsNullOrEmpty(categoryId))
                {
                    throw new ArgumentNullException(nameof(categoryId), "Category ID cannot be null or empty.");
                }

                string url = $"{_apiSettings.Value.BaseUrl}/Photo/0_{categoryId}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to fetch data. Status Code: {response.StatusCode}");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var catRes = JsonConvert.DeserializeObject<ResponseDTO<List<Photos>>>(jsonResponse);

                if (catRes == null || catRes.ResponseData == null || !catRes.ResponseData.Any())
                {
                    return new ResponseDTO<List<Photos>>
                    {
                        IsSuccess = false,
                        Status = 0,
                        ResponseData = new List<Photos>(),
                        Message = "No images found."
                    };
                }

                // Generate image URLs
                foreach (var image in catRes.ResponseData)
                {
                    image.PhotoPath = FileUploadHelper.GenerateFileUrl(image.PhotoPath, httpContextAccessor.HttpContext!.Request);
                }

                return catRes;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred during the notice request: {ex.Message}", ex);
            }
        }

        public async Task<ResponseDTO<List<Photos>>> GetPhotoIdAsync(string pid)
        {
            try
            {
                if (string.IsNullOrEmpty(pid))
                {
                    throw new ArgumentNullException(nameof(pid), "Category ID cannot be null or empty.");
                }

                string url = $"{_apiSettings.Value.BaseUrl}/Photo/2_{pid}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to fetch data. Status Code: {response.StatusCode}");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var catRes = JsonConvert.DeserializeObject<ResponseDTO<List<Photos>>>(jsonResponse);

                return catRes;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred during the notice request: {ex.Message}", ex);
            }
        }

        //youtube section start

        //create photo
        public async Task<ResponseDTO<Photos>> AddYoutubeLInkAsync(Photos photo)
        {
            try
            {
                string url = $"{_apiSettings.Value.BaseUrl}/Photo";
                var jsonNotice = JsonConvert.SerializeObject(photo);
                var content = new StringContent(jsonNotice, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var createdPhotoRes = JsonConvert.DeserializeObject<ResponseDTO<Photos>>(jsonResponse);
                return createdPhotoRes!;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred during the notice request: {ex.Message}", ex);
            }
        }

        //now get all youtube links under a youtube category that we have created already with PCKIDFK = 10014, so under this id will be get only youtube links,
        //because we have not created separate table for youtube  links, we are saving thes links in photos table also
        public async Task<ResponseDTO<List<Photos>>> GetAllYoutubeLinksAsync(string categoryId)
        {
            try
            {
                if (string.IsNullOrEmpty(categoryId))
                {
                    throw new ArgumentNullException(nameof(categoryId), "Category ID cannot be null or empty.");
                }

                string url = $"{_apiSettings.Value.BaseUrl}/Photo/0_{categoryId}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to fetch data. Status Code: {response.StatusCode}");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var catRes = JsonConvert.DeserializeObject<ResponseDTO<List<Photos>>>(jsonResponse);

                if (catRes == null || catRes.ResponseData == null || !catRes.ResponseData.Any())
                {
                    return new ResponseDTO<List<Photos>>
                    {
                        IsSuccess = false,
                        Status = 0,
                        ResponseData = new List<Photos>(),
                        Message = "No images found."
                    };
                }

                ////here i am performing mapping instead of passing whole Photos model i have created separate model for this youtube
                //var links = catRes.ResponseData.Select(y => new YoutubeDTO
                //{
                //    Yid = y.PID,
                //    Title = y.Title,
                //    Description = y.Description,
                //    Date = y.PDate,
                //    Url = y.ThumbNail,
                //});
                //// Generate image URLs
                //foreach (var image in catRes.ResponseData)
                //{
                //    image.PhotoPath = FileUploadHelper.GenerateFileUrl(image.PhotoPath, httpContextAccessor.HttpContext!.Request);
                //}

                return catRes;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred during the notice request: {ex.Message}", ex);
            }
        }





    }
}
