using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SchoolProj.DAL.GalleryRepository;
using SchoolProj.General;
using SchoolProj.Models;
using SchoolProj.Models.Domain;
using SchoolProj.Models.DTO;
using System.Runtime.Intrinsics.X86;

namespace SchoolProj.DAL.PageLoadService
{
    public class PageLoadRepo:IPageLoadRepo
    {
        private List<Page> _pages = null;
        private List<Slider> _slider = null;
        private List<NoticeDTO> _notices  = new List<NoticeDTO> { new NoticeDTO() };
        public List<Photos> _Photos { get; set; } = new List<Photos>();

        public List<Page> Pages
        {
            get
            {
                if (_pages == null)
                {
                    _pages = new List<Page>();  // Initialize the list only when accessed for the first time
                }
                return _pages;
            }
        }

        public List<Slider> Slides
        {
            get
            {
                if(_slider == null)
                    _slider = new List<Slider>();
                return _slider;
            }
        }

        private readonly HttpClient _httpClient;
        private readonly IOptions<ApiSettings> _apiSettings;
        private readonly IHttpContextAccessor httpContext;
        

        public PageLoadRepo(HttpClient httpClient, IOptions<ApiSettings> apiSettings, IHttpContextAccessor httpContext)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings;
            this.httpContext = httpContext;
            
        }

        //GET:api/webpage
        //Initial method :- it will be initially called in program .cs file 
        //whenever you try to update any page dont forget to call this method, so we will call this inside PageRepo's UpdatePageAsync method
        public async Task LoadAllPagesOnceAsync()
        {
            try
            {
                // Only reload pages if the list is not already loaded
                if (_pages == null)
                {
                    // Initialize _pages if it's null
                    _pages = new List<Page>();  // Ensure it's initialized before any usage
                }

                // Set a reasonable timeout for the HTTP client
                _httpClient.Timeout = TimeSpan.FromSeconds(120); // Adjust as needed

                string url = $"{_apiSettings.Value.BaseUrl}/webpage";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();  // Throws if the status code is not 2xx

                var jsonResponse = await response.Content.ReadAsStringAsync();
               // Console.WriteLine("Response JSON: " + jsonResponse); // Log the raw JSON response

                var pagesData = JsonConvert.DeserializeObject<ResponseDTO<List<Page>>>(jsonResponse);

                if (pagesData?.ResponseData != null)
                {
                    _pages = pagesData.ResponseData;  // Assign pages only if valid data is returned
                }
                else
                {
                    Console.WriteLine("No pages data returned.");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HttpRequestException: {ex.Message}");
                throw new Exception($"Error calling the page API: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"TaskCanceledException: {ex.Message}");
                throw new Exception("The operation was canceled, possibly due to a timeout or network issue.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                throw new Exception($"An error occurred during page load: {ex.Message}");
            }
        }



        // Get a page by its ID
        public async Task<Page?> GetPageByIdAsync(string pageId)
        {
            if (_pages == null || !_pages.Any())  // Check if _pages is null or empty
            {
                // You can log here or return null directly
                Console.WriteLine("Pages not loaded or empty.");
                return null;  // Return null if pages are not loaded or available
            }

            var page = _pages.FirstOrDefault(p => p.ID == pageId);  // Find page by ID

            if (page == null)
            {
                Console.WriteLine($"Page with ID {pageId} not found.");
            }
            
            //before sending page data first generate its Image url
            if(page!.PagePic !=null)
            {
                page.PagePic = FileUploadHelper.GenerateFileUrl(page.PagePic, httpContext.HttpContext!.Request);
            }
            return await Task.FromResult(page);  // Return the found page or null
        }

        public async Task LoadAllSlidesOnceAsync()
        {
            try
            {
                // Only reload pages if the list is not already loaded
                if (_slider == null)
                {
                    // Initialize _pages if it's null
                    _slider = new List<Slider>();  // Ensure it's initialized before any usage
                }

                // Set a reasonable timeout for the HTTP client
                _httpClient.Timeout = TimeSpan.FromSeconds(120); // Adjust as needed

                string url = $"{_apiSettings.Value.BaseUrl}/slider";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();  // Throws if the status code is not 2xx

                var jsonResponse = await response.Content.ReadAsStringAsync();
                // Console.WriteLine("Response JSON: " + jsonResponse); // Log the raw JSON response

                var pagesData = JsonConvert.DeserializeObject<ResponseDTO<List<Slider>>>(jsonResponse);

                if (pagesData?.ResponseData != null)
                {
                    _slider = pagesData.ResponseData;  // Assign pages only if valid data is returned
                }
                else
                {
                    Console.WriteLine("No pages data returned.");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HttpRequestException: {ex.Message}");
                throw new Exception($"Error calling the page API: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"TaskCanceledException: {ex.Message}");
                throw new Exception("The operation was canceled, possibly due to a timeout or network issue.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                throw new Exception($"An error occurred during page load: {ex.Message}");
            }
        }

        public  List<Slider> GetSlidersAsync()
        {
            foreach(var slide in _slider)
            {
                slide.SliderImage = FileUploadHelper.GenerateFileUrl(slide.SliderImage!, httpContext.HttpContext!.Request);
            }
            return _slider;
        }

        //get notice list with files
        public async Task GetNoticeListWithFilesAsync()
        {
            try
            {
                // Set a reasonable timeout for the HTTP client
                _httpClient.Timeout = TimeSpan.FromSeconds(120); // Adjust as needed

                string url = $"{_apiSettings.Value.BaseUrl}/notice/2_0";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                // Console.WriteLine("Response JSON: " + jsonResponse); // Logging the raw JSON response

                var noticeData = JsonConvert.DeserializeObject<ResponseDTO<List<NoticeDTO>>>(jsonResponse);

                //return noticeData;
                foreach (var notice in noticeData.ResponseData)
                {
                    if (!string.IsNullOrEmpty(notice.ThumbNail))
                    {
                        var thumbNailUrl = FileUploadHelper.GenerateFileUrl(notice.ThumbNail, httpContext.HttpContext.Request);
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
                        var filePathUrl = FileUploadHelper.GenerateFileUrl(notice.FilePath, httpContext.HttpContext.Request);
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

                //return noticeData;
                _notices = noticeData.ResponseData;
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

        public List<NoticeDTO> GetNotices()
        {
            return _notices;
        }


    }
}
