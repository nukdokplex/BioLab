using BioLab.Database;
using BioLab.Database.DataModels;
using BioLab.Utils;
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

namespace BioLab.UI.Windows
{
    /// <summary>
    /// Логика взаимодействия для AuthTriesList.xaml
    /// </summary>
    public partial class AuthTriesList : Window
    {
        private PaginationController paginationController;
        private user CurrentUser;

        public AuthTriesList()
        {
            InitializeComponent();
            
        }

        private void AuthTriesHistoryUserLoginField_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (!string.IsNullOrWhiteSpace(textBox.Text))
            {
                try
                {
                    CurrentUser = (from user in App.DB.users
                                   where user.login == textBox.Text
                                   select user).Single<user>();
                }
                catch(InvalidOperationException exception)
                {
                    MessageBox.Show("Произошла ошибка при попытке поиска пользователя с таким логином: " + exception.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    textBox.Text = "";
                    CurrentUser.id = -1;
                    return;
                }
                long currentEntriesCount = (from t in App.DB.users_auth_tries
                                            where t.user == CurrentUser.id
                                            select t).Count();
                
                paginationController.EntriesCountChanged(currentEntriesCount);
            }
            else
            {
                MessageBox.Show("Поле логина пользователя не должно быть пустым!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                textBox.Text = "";
                CurrentUser.id = -1;
                return;
            }
        }

        private void AuthTriesOutputCountCombobox_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            try
            {
                long selectedEntriesPerPage = long.Parse((comboBox.SelectedItem as ComboBoxItem).Tag.ToString());

                
            }
            catch(NullReferenceException exception)
            {
                
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            long entriesCount = App.DB.users_auth_tries.Count();
            PaginationControls controls = new PaginationControls(FirstPageButton, PreviousPageButton, NextPageButton, LastPageButton, SpecifyPageTextBox);
            paginationController = new PaginationController(10, entriesCount, controls);
            paginationController.OnRefresh += RefreshEntries;

        }

        private void RefreshEntries(object sender, PaginationOnRefreshEventArgs e)
        {
            List<AuthTryShrinked> authTries;
            if (CurrentUser.id != -1)
            {
                long successfulTriesCount = (from t in App.DB.users_auth_tries
                                             join u in App.DB.users
                                             on t.user equals u.id
                                             where t.user == CurrentUser.id &&
                                             t.is_success == true
                                             select new object()).Count();
                SuccessfulAuthTriesCountTextBox.Text = successfulTriesCount.ToString();

                authTries = (from t in App.DB.users_auth_tries
                             join u in App.DB.users
                             on t.user equals u.id
                             where t.user == CurrentUser.id
                             orderby t.tried_at descending
                             select new AuthTryShrinked
                             {
                                 tried_at = t.tried_at,
                                 login = u.login,
                                 is_successful = t.is_success
                             }
                             ).ToList<AuthTryShrinked>();


            }
            else
            {
                long successfulTriesCount = (from t in App.DB.users_auth_tries
                                             join u in App.DB.users
                                             on t.user equals u.id
                                             select new object()).Count();
                SuccessfulAuthTriesCountTextBox.Text = successfulTriesCount.ToString();
                authTries = (from t in App.DB.users_auth_tries
                             join u in App.DB.users
                             on t.user equals u.id
                             where t.user == CurrentUser.id
                             orderby t.tried_at descending
                             select new AuthTryShrinked
                             {
                                 tried_at = t.tried_at,
                                 login = u.login,
                                 is_successful = t.is_success
                             }
                             ).ToList<AuthTryShrinked>();
                
            }
            AuthTriesHistoryDataGrid.Columns.Clear();
            AuthTriesHistoryDataGrid.ItemsSource = authTries;
        }
    }
}
