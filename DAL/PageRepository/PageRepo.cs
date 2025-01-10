using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SchoolProj.General;
using SchoolProj.Models.Domain;
using SchoolProj.Models;
using System.Text;
using SchoolProj.Models.DTO;
using System.Formats.Tar;
using System.Drawing.Drawing2D;
using SchoolProj.DAL.PageLoadService;

namespace SchoolProj.DAL.PageRepository
{
    public class PageRepo: IPageRepo
    {

        private readonly HttpClient _httpClient;
        private readonly IOptions<ApiSettings> _apiSettings;
        private readonly IHttpContextAccessor httpContext;
        private readonly IPageLoadRepo pageLoadRepo;

        public PageRepo(HttpClient httpClient, IOptions<ApiSettings> apiSettings, IHttpContextAccessor httpContext, IPageLoadRepo pageLoadRepo)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings;
            this.httpContext = httpContext;
            this.pageLoadRepo = pageLoadRepo;
        }


        //POST: api/webpage
        public async Task<ResponseDTO<Page>> CreatePageAsync(Page page)
        {
            try
            {
                // Set a reasonable timeout for the HTTP client
                _httpClient.Timeout = TimeSpan.FromSeconds(120);

                string url = $"{_apiSettings.Value.BaseUrl}/webpage"; // Adjust the endpoint as needed

                // Serialize the notice object to JSON
                var jsonNotice = JsonConvert.SerializeObject(page);

                // Create a StringContent object with the serialized JSON
                var content = new StringContent(jsonNotice, Encoding.UTF8, "application/json");

                // Make the POST request to the API
                HttpResponseMessage response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode(); // Throws an exception if not successful

                // Read the response content as a string
                var jsonResponse = await response.Content.ReadAsStringAsync();

                // Deserialize the response into ResponseDTO<Notice>
                var createdPage = JsonConvert.DeserializeObject<ResponseDTO<Page>>(jsonResponse);
                return createdPage;
            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP request-specific errors
                Console.WriteLine($"HttpRequestException: {ex.Message}");
                throw new Exception($"Error calling notice API: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                // Handle timeouts or cancellation errors
                Console.WriteLine($"TaskCanceledException: {ex.Message}");
                throw new Exception("The operation was canceled, possibly due to a timeout or network issue.");
            }
            catch (JsonException ex)
            {
                // Handle JSON parsing errors
                Console.WriteLine($"JsonException: {ex.Message}");
                throw new Exception("Error parsing JSON response.");
            }
            catch (Exception ex)
            {
                // Handle all other errors
                Console.WriteLine($"Exception: {ex.Message}");
                throw new Exception($"An error occurred during the notice request: {ex.Message}");
            }
        }

        //GET:api/webpage
        public async Task<ResponseDTO<List<Page>>> GetAllPagesAsync()
        {
            try
            {
                // Set a reasonable timeout for the HTTP client
                _httpClient.Timeout = TimeSpan.FromSeconds(120); // Adjust as needed

                string url = $"{_apiSettings.Value.BaseUrl}/webpage";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response JSON: " + jsonResponse); // Logging the raw JSON response

                var pagesData = JsonConvert.DeserializeObject<ResponseDTO<List<Page>>>(jsonResponse);

                return pagesData;
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

        //GET:- api/webpage/0_${id}
        public async Task<ResponseDTO<List<Page>>> GetPageAsync(string id)
        {
            try
            {
                // Set a reasonable timeout for the HTTP client
                _httpClient.Timeout = TimeSpan.FromSeconds(120); // Adjust as needed

                string url = $"{_apiSettings.Value.BaseUrl}/webpage/0_{id}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response JSON: " + jsonResponse); // Logging the raw JSON response

                var pageData = JsonConvert.DeserializeObject<ResponseDTO<List<Page>>>(jsonResponse);
                return pageData!;
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

        //Get page edit model for edit page, because we need to create different model for page edit so map propeties ehre
        public PageEditModel GetPageEditViewModel(Page page)
        {
            var pageEditModel = new PageEditModel
            {
                Id = page.ID!,
                PageTitle = page.PageTitle!,
                PageName = page.PageName!,
                PageData = page.PageData!,
                PageHeading = page.PageHeading!,
                ExistingPagePic = page.PagePic!,
                PageUrl = page.PageUrl!,

            };

            return pageEditModel;
        }

        public async Task<ResponseDTO<Page>> UpdatePageAsync(Page page)
        {
            try
            {
                // Set a reasonable timeout for the HTTP client
                _httpClient.Timeout = TimeSpan.FromSeconds(120);

                string url = $"{_apiSettings.Value.BaseUrl}/webpage/1"; // Adjust the endpoint as needed

                // Serialize the notice object to JSON
                var jsonNotice = JsonConvert.SerializeObject(page);

                // Create a StringContent object with the serialized JSON
                var content = new StringContent(jsonNotice, Encoding.UTF8, "application/json");

                // Make the POST request to the API
                HttpResponseMessage response = await _httpClient.PutAsync(url, content);
                response.EnsureSuccessStatusCode(); // Throws an exception if not successful

                // Read the response content as a string
                var jsonResponse = await response.Content.ReadAsStringAsync();

                // Deserialize the response into ResponseDTO<Notice>
                var createdPage = JsonConvert.DeserializeObject<ResponseDTO<Page>>(jsonResponse);
                //first update pageload in PageLoad Service method
                //await pageLoadRepo.LoadAllPagesOnceAsync();
                return createdPage;
            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP request-specific errors
                Console.WriteLine($"HttpRequestException: {ex.Message}");
                throw new Exception($"Error calling notice API: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                // Handle timeouts or cancellation errors
                Console.WriteLine($"TaskCanceledException: {ex.Message}");
                throw new Exception("The operation was canceled, possibly due to a timeout or network issue.");
            }
            catch (JsonException ex)
            {
                // Handle JSON parsing errors
                Console.WriteLine($"JsonException: {ex.Message}");
                throw new Exception("Error parsing JSON response.");
            }
            catch (Exception ex)
            {
                // Handle all other errors
                Console.WriteLine($"Exception: {ex.Message}");
                throw new Exception($"An error occurred during the notice request: {ex.Message}");
            }
        }


    }
}

