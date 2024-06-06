
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows;

namespace music_streaming_service.MVVM.Model
{
    public class RegistrationRepository
    {
        private readonly HttpClient _httpClient;
        public RegistrationRepository()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7199") };
        }

        public async Task<bool> RegisterUserAsync(string username, string password, string email)
        {
            try
            {
                var user = new { Username = username, Password = password, Email = email };
                var json = JsonConvert.SerializeObject(user);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/auth/register", data);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Регистрация прошла успешно!");
                    return true;
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Ошибка при регистрации: " + errorMessage);
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
