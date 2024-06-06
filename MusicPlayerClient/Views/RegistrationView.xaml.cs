using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using music_streaming_service.MVVM.Model;
using MusicPlayerClient.Extensions;
using MusicPlayerClient.ViewModels;
using MusicPlayerClient.Views.Errors;
using MusicPlayerData.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace MusicPlayerClient.Views
{
    /// <summary>
    /// Логика взаимодействия для RegistrationView.xaml
    /// </summary>
    public partial class RegistrationView : Window
    {
        private IServiceProvider? _serviceProvider;
        public RegistrationView()
        {
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            IServiceCollection services = new ServiceCollection();
            _serviceProvider = services.AddViewModels()
                                       .AddNavigation()
                                       .AddDbContextFactory()
                                       .AddStores()
                                       .AddServices()
                                       .BuildServiceProvider();
            IDbContextFactory<DataContext> dbFactory = _serviceProvider.GetRequiredService<IDbContextFactory<DataContext>>();
            Directory.CreateDirectory("data");
            InitializeComponent();
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.WindowState != WindowState.Maximized)
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
                if (sender is Button button)
                    button.Content = "❐";
            }
            else
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
                if (sender is Button button)
                    button.Content = "▢";
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void AppTopBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        public void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.BorderThickness = new Thickness(6);
            }
            else
            {
                this.BorderThickness = new Thickness(0);
            }
        }

        private void Label_Click(object sender, MouseButtonEventArgs e)
        {
            Views.LoginView login = new Views.LoginView();
            login.Show();
            this.Close();
        }

        private async void Reg_Click(object sender, RoutedEventArgs e)
        {
            string username = LoginTb.Text;
            string password = PasswordTb.Password;
            string email = EmailTb.Text;

            // Проверка валидности данных
            if (!ValidateInput(username, password, email))
                return;

            var registerRepository = new RegistrationRepository();
            var loginRepository = new LoginRepository();

            try
            {
                bool isRegistered = await registerRepository.RegisterUserAsync(username, password, email);

                if (isRegistered)
                {
                    bool isAuthenticated = await loginRepository.AuthenticateUserAsync(username, password);

                    if (isAuthenticated)
                    {
                        MainWindow main = new MainWindow()
                        {
                            DataContext = _serviceProvider.GetRequiredService<MainViewModel>()
                        };
                        main.Show();
                        this.Close();
                    }
                    else
                    {
                        ShowError("Ошибка аутентификации. Проверьте правильность введенных данных.");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка при аутентификации: {ex.Message}");
            }
        }

        private bool ValidateInput(string username, string password, string email)
        {
            bool isValid = true;

            // Проверка наличия данных
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(email))
            {
                ShowError("Не все данные были введены!");
                isValid = false;
            }
            // Проверка длины и содержания пароля
            else if (!IsValidPassword(password))
            {
                ShowError("Пароль должен быть не менее 8 символов, содержать хотя бы одну заглавную букву и одну цифру!");
                isValid = false;
            }
            // Проверка валидности email
            else if (!IsValidEmail(email))
            {
                ShowError("Введите правильную почту!");
                isValid = false;
            }

            return isValid;
        }

        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            string pattern = @"^(?=.*[A-Z])(?=.*\d)[A-Za-z\d]{8,}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(password);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void ShowError(string errorMessage)
        {
            var errorWindow = new ErrorWindow(errorMessage);
            errorWindow.ShowDialog();
        }
    }
}
