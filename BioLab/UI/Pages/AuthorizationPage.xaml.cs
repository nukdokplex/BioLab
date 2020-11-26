using BioLab.Database;
using BioLab.UI.Windows;
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

        public static void SaveAuthTry(user u, bool is_success)
        {
            users_auth_tries currentTry = new users_auth_tries();
            currentTry.is_success = is_success;
            currentTry.tried_at = DateTime.Now;
            currentTry.user = u.id;
            //currentTry.id = App.DB.users_auth_tries.Count() + 1;
            App.DB.users_auth_tries.Add(currentTry);
            App.DB.SaveChanges();
        }

        public static user TryFoundUserByLogin(string login)
        {
            try
            {

                return (from user in App.DB.users
                 where user.login == login
                 select user).Single();

            }
            catch (InvalidOperationException exception)
            {
                user u = new user();
                u.id = -1;
                return u;
            }
            
        }

        private void authButton_Click(object sender, RoutedEventArgs e)
        {
            List<user> foundUsers = new List<user>();
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

                user u = TryFoundUserByLogin(loginField.Text);
                if (u.id != -1)
                {
                    SaveAuthTry(u, false);
                }
                
                MessageBox.Show("Нет пользователя с таким логином и (или) паролем: "+exception.Message, "Ошибка при входе", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SaveAuthTry(App.currentUser, true);

            NavigationService.Navigate(new StartPage());
            
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow.header.HeaderTitle.Text = Title;
        }

        private void OpenAuthTriesWindowsButton_Click(object sender, RoutedEventArgs e)
        {
            (new AuthTriesList()).Show();
        }
    }
}
