using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SchoolProj.General;
using SchoolProj.Models;
using SchoolProj.Models.AccountDTO;
using SchoolProj.Models.Domain;
using SchoolProj.Models.DTO;
using System.Text;

namespace SchoolProj.DAL.UserRepository
{
    public class UserRepo: IUserRepo
    {

        private readonly HttpClient _httpClient;
        private readonly IOptions<ApiSettings> _apiSettings;
        private readonly IHttpContextAccessor httpContextAccessor;


        public UserRepo(HttpClient httpClient, IOptions<ApiSettings> apiSettings, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(120); // Set timeout once during initialization
            _apiSettings = apiSettings;
            this.httpContextAccessor = httpContextAccessor;
        }


        //Put:method for password change api will be api/User/0
        public async Task<Response> PasswordChangeAsync(PasswordChangeDTO passwordObject)
        {
            try
            {
                var user = new User
                {
                    OldPassword = passwordObject.OldPassword,
                    UserPassword = passwordObject.NewPassword,
                    UserName = passwordObject.Username
                };
                // Construct the URL
                string url = $"{_apiSettings.Value.BaseUrl}/User/0";  // 1 is for gyne opd patient registration'
                // Serialize the login object to JSON
                var data = JsonConvert.SerializeObject(user);
                // Create the HTTP content with the serialized JSON
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                // Make the API request
                HttpResponseMessage response = await _httpClient.PutAsync(url, content);
                // Ensure the request was successful
                response.EnsureSuccessStatusCode();
                // Read the response content as a string
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var loginResponse2 = JsonConvert.DeserializeObject<Response>(jsonResponse);
                return loginResponse2!;
            }
            catch (HttpRequestException ex)
            {
                // Handle specific HTTP errors
                throw new Exception($"Error calling password change API: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle general errors
                throw new Exception($"An error occurred during password change: {ex.Message}");
            }
        }


    }
}
