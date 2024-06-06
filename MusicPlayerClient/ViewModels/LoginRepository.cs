using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.Windows;

namespace music_streaming_service.MVVM.Model
{
    public class LoginRepository
    {
        private readonly HttpClient _httpClient;
        public LoginRepository()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7199");
        }
        public async Task<bool> AuthenticateUserAsync(string username, string password)
        {
            try
            {
                var loginModel = new
                {
                    Username = username,
                    Password = password
                };

                var json = JsonConvert.SerializeObject(loginModel);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var responce = await _httpClient.PostAsync("/api/auth/login", data);

                if (responce.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorMessage = await responce.Content.ReadAsStringAsync();
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
                return false;
            }
        }
    }
}
