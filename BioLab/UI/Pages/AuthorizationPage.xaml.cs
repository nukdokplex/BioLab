using BioLab.Database;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BioLab.UI.Pages
{
    /// <summary>
    /// Логика взаимодействия для StartPage.xaml
    /// </summary>
    public partial class AuthorizationPage : Page
    {
         
        public AuthorizationPage()
        {
            InitializeComponent();
        }

        private void authButton_Click(object sender, RoutedEventArgs e)
        {
            if (loginField.Text == String.Empty || passwordField.Password == String.Empty)
            {
                MessageBox.Show("Все поля должны быть заполнены", "Ошибка при входе", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                App.currentUser = (from user in App.DB.users
                                   where user.login == loginField.Text &&
                                   user.password == passwordField.Password
                                   select user).Single<user>();
            }
            catch (InvalidOperationException exception)
            {
                MessageBox.Show("Нет пользователя с таким логином и (или) паролем: "+exception.Message, "Ошибка при входе", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            NavigationService.Navigate(new StartPage());
            
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow.header.HeaderTitle.Text = Title;
        }
    }
}
