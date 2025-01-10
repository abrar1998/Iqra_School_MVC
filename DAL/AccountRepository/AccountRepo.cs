using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using SchoolProj.General;
using SchoolProj.Models.AccountDTO;
using SchoolProj.Models.Domain;
using System.Net.Http;
using System.Text;

namespace SchoolProj.DAL.AccountRepository
{
    public class AccountRepo:IAccountRepo
    {


        private readonly HttpClient _httpClient;
        private readonly IOptions<ApiSettings> _apiSettings;
        private readonly IConfiguration configuration;

        public AccountRepo(HttpClient httpClient, IOptions<ApiSettings> apiSettings, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings;
            this.configuration = configuration;
        }

        // Login API Call
        //POST: api/Login
        public async Task<User> Login(LoginDTO login)
        {
            try
            {
                // Construct the URL
                string url = $"{_apiSettings.Value.BaseUrl}/Login";  // Assuming the endpoint is 'Login'
                                                                     // Access the APIKEY from appsettings.json
                string apiKey = configuration["ApiSettings:APIKEY"]!;
                // Create the login request payload
                var loginsend = new LoginDomain()
                {
                    userName = login.UserName,
                    userPassword = login.UserPassword,
                    //Token = apiKey
                };

                // Serialize the login object to JSON
                var data = JsonConvert.SerializeObject(loginsend);

                // Create the HTTP content with the serialized JSON
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                // Make the API request
                HttpResponseMessage response = await _httpClient.PostAsync(url, content);

                // Ensure the request was successful
                response.EnsureSuccessStatusCode();

                // Read the response content as a string
                var jsonResponse = await response.Content.ReadAsStringAsync();

                // Deserialize the response to a LoginResponse object
                var loginResponse = JsonConvert.DeserializeObject<User>(jsonResponse);

                return loginResponse!;
            }
            catch (HttpRequestException ex)
            {
                // Handle specific HTTP errors
                throw new Exception($"Error calling login API: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle general errors
                throw new Exception($"An error occurred during login: {ex.Message}");
            }
        }
    }

}

