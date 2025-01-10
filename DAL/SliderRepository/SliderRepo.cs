using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SchoolProj.General;
using SchoolProj.Models;
using SchoolProj.Models.Domain;
using SchoolProj.Models.DTO;
using System.Text;

namespace SchoolProj.DAL.SliderRepository
{
    public class SliderRepo : ISliderRepo
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<ApiSettings> _apiSettings;
        private readonly IHttpContextAccessor httpContextAccessor;

        //private readonly IHttpContextAccessor _httpContext;
        private static List<Slider> _SliderList = new List<Slider>();

        //public SliderRepo(HttpClient httpClient, IOptions<ApiSettings> apiSettings, IHttpContextAccessor httpContext)
        //{
        //    _httpClient = httpClient;
        //    _apiSettings = apiSettings;
        //    _httpContext = httpContext;
        //}

        public SliderRepo(HttpClient httpClient, IOptions<ApiSettings> apiSettings, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(120); // Set timeout once during initialization
            _apiSettings = apiSettings;
            this.httpContextAccessor = httpContextAccessor;
        }


        // POST: api/slider
        public async Task<ResponseDTO<Slider>> CreateSliderAsync(Slider slider)
        {
            try
            {
                // Set a reasonable timeout for the HTTP client
                //_httpClient.Timeout = TimeSpan.FromSeconds(120);
                string url = $"{_apiSettings.Value.BaseUrl}/slider";

                var jsonNotice = JsonConvert.SerializeObject(slider);
                var content = new StringContent(jsonNotice, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var createdSlider = JsonConvert.DeserializeObject<ResponseDTO<Slider>>(jsonResponse);
                return createdSlider!;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred during the notice request: {ex.Message}", ex);
            }
        }

        // GET: api/slider
        public async Task<ResponseDTO<List<Slider>>> GetSlidesAsync()
        {
            try
            {
                // Set a reasonable timeout for the HTTP client
                //_httpClient.Timeout = TimeSpan.FromSeconds(120);
                string url = $"{_apiSettings.Value.BaseUrl}/slider";

                HttpResponseMessage response = await _httpClient.GetAsync(url);
                //using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
                //HttpResponseMessage response = await _httpClient.GetAsync(url, cts.Token); ;
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var slides = JsonConvert.DeserializeObject<ResponseDTO<List<Slider>>>(jsonResponse);
                if(slides.ResponseData !=null)
                {
                    _SliderList = slides.ResponseData;
                }
                return slides!;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred during the notice request: {ex.Message}", ex);
            }
        }

        public List<Slider> GetSlidesFromList()
        {
            return _SliderList;
        }

        // DELETE: api/slider/1 
        //public async Task<ResponseDTO<Slider>> DeleteSliderAsync(Slider slider)
        //{
        //    try
        //    {
        //        // Use a separate instance of HttpClient
        //      //  using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(120) };
        //        _httpClient.Timeout = TimeSpan.FromSeconds(120);
        //        string url = $"{_apiSettings.Value.BaseUrl}/slider/1";
        //        var jsonNotice = JsonConvert.SerializeObject(slider);
        //        var content = new StringContent(jsonNotice, Encoding.UTF8, "application/json");

        //        HttpResponseMessage response = await _httpClient.PutAsync(url,content);
        //        response.EnsureSuccessStatusCode();

        //        var jsonResponse = await response.Content.ReadAsStringAsync();
        //        var deletedSlide = JsonConvert.DeserializeObject<ResponseDTO<Slider>>(jsonResponse);
        //        return deletedSlide!;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"An error occurred during the slider request: {ex.Message}", ex);
        //    }
        //}

        // Delete Slide by ID

        public async Task<ResponseDTO<Slider>> DeleteSliderAsync(Slider slider)
        {
            try
            {
                string url = $"{_apiSettings.Value.BaseUrl}/slider/1";
                var jsonNotice = JsonConvert.SerializeObject(slider);
                var content = new StringContent(jsonNotice, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PutAsync(url, content);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var deletedSlide = JsonConvert.DeserializeObject<ResponseDTO<Slider>>(jsonResponse);
                return deletedSlide!;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred during the slider request: {ex.Message}", ex);
            }
        }

        public async Task<ResponseDTO<Slider>> DeleteSlideByIdAsync(string sId)
        {
            try
            {
                var slides = _SliderList;
                var slideToDelete = slides.FirstOrDefault(s => s.SliderId == sId);

                if (slideToDelete == null)
                {
                    throw new Exception("Slide not found");
                }
                //now send the slide you want to delete
                var res = await DeleteSliderAsync(slideToDelete);
                return res;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred during the delete request: {ex.Message}", ex);
            }
        }

        // Get Slide by ID
        public Slider GetSlideByIdAsync(string sId)
        {
            try
            {
                //_SliderList is declared as static variable, this service is singleton
                var slide = _SliderList.FirstOrDefault(s => s.SliderId == sId);
                return slide!;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred during the get request: {ex.Message}", ex);
            }
        }


        public async Task<ResponseDTO<List<NoticeDTO>>> GetNoticeListWithFilesAsync()
        {
            try
            {
                // Set a reasonable timeout for the HTTP client
           
                string url = $"{_apiSettings.Value.BaseUrl}/notice/2_0";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                // Console.WriteLine("Response JSON: " + jsonResponse); // Logging the raw JSON response

                var noticeData = JsonConvert.DeserializeObject<ResponseDTO<List<NoticeDTO>>>(jsonResponse);

                //return noticeData;
               if(noticeData.ResponseData.Count() > 0)
                {
                    foreach (var notice in noticeData.ResponseData)
                    {
                        if (!string.IsNullOrEmpty(notice.ThumbNail))
                        {
                            var thumbNailUrl = FileUploadHelper.GenerateFileUrl(notice.ThumbNail, httpContextAccessor.HttpContext.Request);
                            if (thumbNailUrl != null)
                            {
                                notice.ThumbNail = thumbNailUrl;
                            }
                            else
                            {
                                //_logger.LogWarning("Thumbnail file not found: {ThumbNail}", notice.ThumbNail);
                            }
                        }

                        if (!string.IsNullOrEmpty(notice.FilePath))
                        {
                            var filePathUrl = FileUploadHelper.GenerateFileUrl(notice.FilePath, httpContextAccessor.HttpContext.Request);
                            if (filePathUrl != null)
                            {
                                notice.FilePath = filePathUrl;
                            }
                            else
                            {
                                //_logger.LogWarning("File not found: {FilePath}", notice.FilePath);
                            }
                        }
                    }
                }

                return noticeData;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HttpRequestException: {ex.Message}");
                throw new Exception($"Error calling notice API: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"TaskCanceledException: {ex.Message}");
                throw new Exception("The operation was canceled, possibly due to a timeout or network issue.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                throw new Exception($"An error occurred during notice request: {ex.Message}");
            }
        }


    }
}
