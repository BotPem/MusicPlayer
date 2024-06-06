using System;
using System.Collections.Generic;
using System.Linq;
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

namespace MusicPlayerClient.Views.Errors
{
    /// <summary>
    /// Логика взаимодействия для LoginErrorWindow.xaml
    /// </summary>
    public partial class LoginErrorWindow : Window
    {
        public LoginErrorWindow(string errorMessage)
        {
            InitializeComponent();
            ErrorMessageTextBlock.Text = errorMessage;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
