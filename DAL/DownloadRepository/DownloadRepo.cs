using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SchoolProj.General;
using SchoolProj.Models.Domain;
using SchoolProj.Models;
using System.Text;
using SchoolProj.Models.DTO;

namespace SchoolProj.DAL.DownloadRepository
{
    public class DownloadRepo: IDownloadRepo
    {

        private readonly HttpClient _httpClient;
        private readonly IOptions<ApiSettings> _apiSettings;
        private readonly IHttpContextAccessor httpContext;

        public DownloadRepo(HttpClient httpClient, IOptions<ApiSettings> apiSettings, IHttpContextAccessor httpContext)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings;
            this.httpContext = httpContext;
        }

        //POST: api/downloads
        //public async Task<ResponseDTO<Downloads>> CreateDownloadsAsync(Downloads download)
        //{
        //    try
        //    {
        //        // Set a reasonable timeout for the HTTP client
        //        _httpClient.Timeout = TimeSpan.FromSeconds(120);

        //        string url = $"{_apiSettings.Value.BaseUrl}/downloads"; // Adjust the endpoint as needed

        //        // Serialize the notice object to JSON
        //        var jsonNotice = JsonConvert.SerializeObject(download);

        //        // Create a StringContent object with the serialized JSON
        //        var content = new StringContent(jsonNotice, Encoding.UTF8, "application/json");

        //        // Make the POST request to the API
        //        HttpResponseMessage response = await _httpClient.PostAsync(url, content);
        //        response.EnsureSuccessStatusCode(); // Throws an exception if not successful

        //        // Read the response content as a string
        //        var jsonResponse = await response.Content.ReadAsStringAsync();

        //        // Deserialize the response into ResponseDTO<Notice>
        //        var createdDownload = JsonConvert.DeserializeObject<ResponseDTO<Downloads>>(jsonResponse);
        //        return createdDownload!;
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

        //GET; api/downloads/1_{did}

        public async Task<ResponseDTO<Downloads>> CreateDownloadsAsync(Downloads download)
        {
            try
            {
                // Create a new HttpClient instance (not recommended for reuse)
                using (var httpClient = new HttpClient())
                {
                    // Set a reasonable timeout for the HTTP client
                    httpClient.Timeout = TimeSpan.FromSeconds(120);

                    // Define the URL for the POST request
                    string url = $"{_apiSettings.Value.BaseUrl}/downloads"; // Adjust the endpoint as needed

                    // Serialize the download object to JSON
                    var jsonNotice = JsonConvert.SerializeObject(download);

                    // Create a StringContent object with the serialized JSON
                    var content = new StringContent(jsonNotice, Encoding.UTF8, "application/json");

                    // Make the POST request to the API
                    HttpResponseMessage response = await httpClient.PostAsync(url, content);
                    response.EnsureSuccessStatusCode(); // Throws an exception if not successful

                    // Read the response content as a string
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    // Deserialize the response into ResponseDTO<Downloads>
                    var createdDownload = JsonConvert.DeserializeObject<ResponseDTO<Downloads>>(jsonResponse);

                    // Return the created download response
                    return createdDownload!;
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP request-specific errors
                Console.WriteLine($"HttpRequestException: {ex.Message}");
                throw new Exception($"Error calling downloads API: {ex.Message}");
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
                throw new Exception($"An error occurred during the downloads request: {ex.Message}");
            }
        }

        public async Task<ResponseDTO<Downloads>> GetNoticeByIdAsync(string dId)
        {
            try
            {
                // Set a reasonable timeout for the HTTP client
                _httpClient.Timeout = TimeSpan.FromSeconds(120); // Adjust as needed

                string url = $"{_apiSettings.Value.BaseUrl}/downloads/1_{dId}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response JSON: " + jsonResponse); // Logging the raw JSON response

                var downloadData = JsonConvert.DeserializeObject<ResponseDTO<Downloads>>(jsonResponse);

                return downloadData!;
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

        //GET: api/downloads/0_,{cid},{Dtype}
        public async Task<ResponseDTO<List<Downloads>>> GetDownloadForStudent(string dId, string dType)
        {
            try
            {
                // Set a reasonable timeout for the HTTP client
                _httpClient.Timeout = TimeSpan.FromSeconds(120); // Adjust as needed

                string url = $"{_apiSettings.Value.BaseUrl}/downloads/0_{dType},{dId}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response JSON: " + jsonResponse); // Logging the raw JSON response

                var downloadData = JsonConvert.DeserializeObject<ResponseDTO<List<Downloads>>>(jsonResponse);

                return downloadData!;
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

        //GET: api/downloads/2_0 get all downlaods
        public async Task<ResponseDTO<List<Downloads>>> GetAllDownloadsAsync()
        {
            try
            {
                // Set a reasonable timeout for the HTTP client
                _httpClient.Timeout = TimeSpan.FromSeconds(120); // Adjust as needed

                string url = $"{_apiSettings.Value.BaseUrl}/downloads/2_0";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response JSON: " + jsonResponse); // Logging the raw JSON response

                var downloadData = JsonConvert.DeserializeObject<ResponseDTO<List<Downloads>>>(jsonResponse);

                return downloadData!;
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

        //GET; GET download by id url = api/downloads/1_{id}
        public async Task<ResponseDTO<List<Downloads>>> GetDownloadAsync(string dId)
        {
            try
            {
                // Set a reasonable timeout for the HTTP client
                _httpClient.Timeout = TimeSpan.FromSeconds(120); // Adjust as needed
                string url = $"{_apiSettings.Value.BaseUrl}/downloads/1_{dId}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response JSON: " + jsonResponse); // Logging the raw JSON response
                var downloadData = JsonConvert.DeserializeObject<ResponseDTO<List<Downloads>>>(jsonResponse);
                return downloadData!;
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

        //detelet download api will be //PUT:api/download/2_0
        public async Task<ResponseDTO<Downloads>> DeleteDownloadAsync(Downloads download)
        {
            try
            {
                // Set a reasonable timeout for the HTTP client
                //_httpClient.Timeout = TimeSpan.FromSeconds(120);

                using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(120) };

                string url = $"{_apiSettings.Value.BaseUrl}/downloads/2_0"; // Adjust the endpoint as needed

                // Serialize the notice object to JSON
                var jsonNotice = JsonConvert.SerializeObject(download);

                // Create a StringContent object with the serialized JSON
                var content = new StringContent(jsonNotice, Encoding.UTF8, "application/json");

                // Make the POST request to the API
                HttpResponseMessage response = await httpClient.PutAsync(url, content);
                response.EnsureSuccessStatusCode(); // Throws an exception if not successful

                // Read the response content as a string
                var jsonResponse = await response.Content.ReadAsStringAsync();

                // Deserialize the response into ResponseDTO<Notice>
                var deletedDownload = JsonConvert.DeserializeObject<ResponseDTO<Downloads>>(jsonResponse);
                return deletedDownload!;
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

        public async Task<ResponseDTO<List<Downloads>>> GetAllDownloadsTypesForDropDownAsync()
        {
            try
            {
                // Set a reasonable timeout for the HTTP client
                _httpClient.Timeout = TimeSpan.FromSeconds(120); // Adjust as needed

                string url = $"{_apiSettings.Value.BaseUrl}/downloads/";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response JSON: " + jsonResponse); // Logging the raw JSON response

                var downloadData = JsonConvert.DeserializeObject<ResponseDTO<List<Downloads>>>(jsonResponse);

                return downloadData!;
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

        public async Task<ResponseDTO<List<Downloads>>> GetAllClassesForDropDownAsync()
        {
            try
            {
                // Create a new HttpClient instance (not recommended for reuse)
                using (var httpClient = new HttpClient())
                {
                    // Set a reasonable timeout for the HTTP client
                    httpClient.Timeout = TimeSpan.FromSeconds(120); // Adjust as needed

                    // Make sure your base URL is correct
                    string url = $"{_apiSettings.Value.BaseUrl}/downloads/3_0";

                    // Send a GET request to the API endpoint
                    HttpResponseMessage response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode(); // Ensure success or throw an exception

                    // Read the response content as a string
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Response JSON: " + jsonResponse); // Logging the raw JSON response

                    // Deserialize the JSON response into the ResponseDTO object
                    var downloadData = JsonConvert.DeserializeObject<ResponseDTO<List<Downloads>>>(jsonResponse);

                    return downloadData!;
                }
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
