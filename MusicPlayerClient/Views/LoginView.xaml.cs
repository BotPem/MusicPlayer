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
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MusicPlayerClient.Views
{
    /// <summary>
    /// Логика взаимодействия для LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        private IServiceProvider? _serviceProvider;
        private readonly LoginRepository _loginRepository;
        public LoginView()
        {
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            _loginRepository = new LoginRepository();
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
            Views.RegistrationView reg = new Views.RegistrationView();
            reg.Show();
            this.Close();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = UserNameTb.Text;
            string password = PasswordTb.Password;

            try
            {
                bool isAuthenticated = await _loginRepository.AuthenticateUserAsync(username, password);

                if (isAuthenticated)
                {
                    MainWindow main = new MainWindow()
                    {
                        DataContext = _serviceProvider.GetRequiredService<MainViewModel>()
                    };
                    main.Show();
                    Application.Current.MainWindow.Close();
                }
                else
                {
                    // Ошибка аутентификации
                    string errorMessage = "Неверные учетные данные или пользователь не найден.";
                    LoginErrorWindow loginErrorWindow = new LoginErrorWindow(errorMessage);
                    loginErrorWindow.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException httpException && httpException.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // Если получили код 401 Unauthorized, то не показываем сообщение об ошибке
                    return;
                }

                // Обработка других исключений
                MessageBox.Show($"Произошла ошибка при аутентификации: {ex.Message}");
            }
        }

    }
}
