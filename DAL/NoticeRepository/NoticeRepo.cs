using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SchoolProj.General;
using SchoolProj.Models;
using SchoolProj.Models.Domain;
using SchoolProj.Models.DTO;
using System.Globalization;
using System.Text;

namespace SchoolProj.DAL.NoticeRepository
{
    public class NoticeRepo: INoticeRepo
    {


        private readonly HttpClient _httpClient;
        private readonly IOptions<ApiSettings> _apiSettings;
        private readonly IHttpContextAccessor httpContext;

        public NoticeRepo(HttpClient httpClient, IOptions<ApiSettings> apiSettings, IHttpContextAccessor httpContext)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings;
            this.httpContext = httpContext;
        }

        //gel all notice GET:- api/notice/2_0
        public async Task<ResponseDTO<List<NoticeDTO>>> GetNoticeListAsync()
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

        //Get:category list api/notice
        public async Task<ResponseDTO<List<Notice>>> GetCategoryListAsync()
        {
            try
            {
                // Set a reasonable timeout for the HTTP client
                _httpClient.Timeout = TimeSpan.FromSeconds(120); // Adjust as needed

                string url = $"{_apiSettings.Value.BaseUrl}/notice";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response JSON: " + jsonResponse); // Logging the raw JSON response

                var categoryData = JsonConvert.DeserializeObject<ResponseDTO<List<Notice>>>(jsonResponse);

                return categoryData;
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


        //add notice POST:- api/notice, inside property ActionType = 1
        public async Task<ResponseDTO<Notice>> CreateNoticeAsync(Notice notice)
        {
            try
            {
                notice.NDate = Convert.ToDateTime(notice.NDate);
                // Set a reasonable timeout for the HTTP client
                _httpClient.Timeout = TimeSpan.FromSeconds(120);

                string url = $"{_apiSettings.Value.BaseUrl}/notice"; // Adjust the endpoint as needed

                // Serialize the notice object to JSON
                var jsonNotice = JsonConvert.SerializeObject(notice);

                // Create a StringContent object with the serialized JSON
                var content = new StringContent(jsonNotice, Encoding.UTF8, "application/json");

                // Make the POST request to the API
                HttpResponseMessage response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode(); // Throws an exception if not successful

                // Read the response content as a string
                var jsonResponse = await response.Content.ReadAsStringAsync();

                // Deserialize the response into ResponseDTO<Notice>
                var createdNotice = JsonConvert.DeserializeObject<ResponseDTO<Notice>>(jsonResponse);
                return createdNotice;
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


        //get notification by id :- api/notice/1_{id}
        public async Task<ResponseDTO<NoticeDTO>> GetNoticeByIdAsync(string nId)
        {
            try
            {
                // Set a reasonable timeout for the HTTP client
                _httpClient.Timeout = TimeSpan.FromSeconds(120); // Adjust as needed

                string url = $"{_apiSettings.Value.BaseUrl}/notice/1_{nId}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response JSON: " + jsonResponse); // Logging the raw JSON response

                var noticeData = JsonConvert.DeserializeObject<ResponseDTO<NoticeDTO>>(jsonResponse);

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

        public async Task<ResponseDTO<NoticeDTO>> DeleteNoticeByIdAsync(NoticeDTO currentNotice, string nId)
        {
            try
            {
                //var currentNoticeResponse = await GetNoticeByIdAsync(nId);
                //var currentNotice = currentNoticeResponse.ResponseData;
                //now delete
                var notice = new Notice
                {
                    Nid = currentNotice!.Nid, //must pass this
                    FilePath = currentNotice.FilePath,
                    Title = currentNotice.Title,
                    Description = currentNotice.Description,
                    Url=currentNotice.Url,
                    Ncid = currentNotice.Ncid,
                    UserName = currentNotice.UserName,
                    ActionType = Convert.ToInt32(currentNotice.ActionType),
                    ThumbNail = currentNotice.ThumbNail,
                    NcName = currentNotice.NcName,
                    NDate = Convert.ToDateTime(currentNotice.NDate),
                    IsFile = Convert.ToInt32(currentNotice.IsFile)
                };
                var response = await DeleteNoticeAsync(notice!);
                return response;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        //DELETE:- api/Notice/2 ,, but for this we dont need to use http delete method , but http put and send notice object
        //public async Task<ResponseDTO<NoticeDTO>> DeleteNoticeAsync(Notice notice)
        //{
        //    try
        //    {
        //        var noticeDto = new NoticeDTO
        //        {
        //            Nid = notice.Nid,
        //            Title = notice.Title,
        //            Description = notice.Description,
        //            Url = notice.Url,
        //            Ncid = notice.Ncid,
        //            UserName = notice.UserName,
        //            IsFile = notice.IsFile.ToString(),
        //            FilePath = notice.FilePath,
        //            ThumbNail = notice.ThumbNail,
        //            NDate = notice.NDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) // Format date


        //        };

        //        //if (notice.NDate !=null)
        //        //{
        //        //    noticeDto.NDate = notice.NDate.Value.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
        //        //}



        //        // Create a new HttpClient instance for this request
        //        using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(120) };

        //        string url = $"{_apiSettings.Value.BaseUrl}/notice/2"; // Adjust the endpoint as needed

        //        // Serialize the notice object to JSON
        //        var jsonNotice = JsonConvert.SerializeObject(noticeDto);

        //        // Create a StringContent object with the serialized JSON
        //        var content = new StringContent(jsonNotice, Encoding.UTF8, "application/json");

        //        // Make the POST request to the API
        //        HttpResponseMessage response = await httpClient.PutAsync(url, content);
        //        response.EnsureSuccessStatusCode(); // Throws an exception if not successful

        //        // Read the response content as a string
        //        var jsonResponse = await response.Content.ReadAsStringAsync();

        //        // Deserialize the response into ResponseDTO<NoticeDTO>
        //        var createdNotice = JsonConvert.DeserializeObject<ResponseDTO<NoticeDTO>>(jsonResponse);
        //        return createdNotice;
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        // Handle HTTP request-specific errors
        //        Console.WriteLine($"HttpRequestException: {ex.Message}");
        //        throw new Exception($"Error calling notice API: {ex.Message}");
        //    }
        //    catch (TaskCanceledException ex)
        //    {
        //        // Handle timeouts or cancellation errors
        //        Console.WriteLine($"TaskCanceledException: {ex.Message}");
        //        throw new Exception("The operation was canceled, possibly due to a timeout or network issue.");
        //    }
        //    catch (JsonException ex)
        //    {
        //        // Handle JSON parsing errors
        //        Console.WriteLine($"JsonException: {ex.Message}");
        //        throw new Exception("Error parsing JSON response.");
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle all other errors
        //        Console.WriteLine($"Exception: {ex.Message}");
        //        throw new Exception($"An error occurred during the notice request: {ex.Message}");
        //    }
        //}


        public async Task<ResponseDTO<NoticeDTO>> DeleteNoticeAsync(Notice notice)
        {
            try
            {
                var noticeDto = new NoticeDTO
                {
                    Nid = notice.Nid,
                    Title = notice.Title,
                    Description = notice.Description,
                    Url = notice.Url,
                    Ncid = notice.Ncid,
                    UserName = notice.UserName,
                    IsFile = notice.IsFile.ToString(),
                    FilePath = notice.FilePath,
                    ThumbNail = notice.ThumbNail,
                    NDate = notice.NDate?.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture) // Explicit formatting
                };

                using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(120) };
                string url = $"{_apiSettings.Value.BaseUrl}/notice/2";

                var jsonSettings = new JsonSerializerSettings
                {
                    DateFormatString = "dd-MM-yyyy",
                    Culture = CultureInfo.InvariantCulture
                };

                var jsonNotice = JsonConvert.SerializeObject(noticeDto, jsonSettings);

                // Log serialized payload for debugging
                Console.WriteLine($"Serialized JSON: {jsonNotice}");

                var content = new StringContent(jsonNotice, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PutAsync(url, content);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ResponseDTO<NoticeDTO>>(jsonResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }


        //get notice list with files
        public async Task<ResponseDTO<List<NoticeDTO>>> GetNoticeListWithFilesAsync()
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
