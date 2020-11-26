using BioLab.Database;
using BioLab.Database.DataModels;
using BioLab.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            CurrentUser = new user();
            CurrentUser.id = -1;
        }

        private void AuthTriesHistoryUserLoginField_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
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
                    catch (InvalidOperationException exception)
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
        }

        private void AuthTriesOutputCountCombobox_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            try
            {
                int selectedEntriesPerPage = int.Parse((comboBox.SelectedItem as ComboBoxItem).Tag.ToString());
                paginationController.EntriesPerPageCountChanged(selectedEntriesPerPage);
                
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
            paginationController.EntriesCountChanged(entriesCount);

        }

        private void RefreshEntries(object sender, PaginationOnRefreshEventArgs e)
        {
            var query = (from t in App.DB.users_auth_tries
                         join u in App.DB.users
                         on t.user equals u.id
                         select new { 
                            t.id,
                            t.is_success,
                            t.tried_at,
                            t.user,
                            u.login
                         });
            
            if (e.EntriesCount != -1)
            {
                //<e.EntriesCount> entries
                query = query.Skip(unchecked((int)e.CurrentOffset)).Take(unchecked((int)e.EntriesCount));
            }

            if (CurrentUser.id != -1)
            {
                query = query.Where(t => t.user == CurrentUser.id);
            }

            if (TriedAtDatePicker.SelectedDate.HasValue)
            {
                query = query.Where(t => DbFunctions.DiffDays(TriedAtDatePicker.SelectedDate.Value, t.tried_at) == 0);
            }

            AuthTriesHistoryDataGrid.Columns.Clear();

            AuthTriesHistoryDataGrid.ItemsSource = query.Select(t => new AuthTryShrinked
            {
                login = t.login,
                tried_at = t.tried_at,
                is_successful = t.is_success
            }).ToList();

            UpdateAuthTriesCounters();

            e.Controls.SetEnabledState(true);
        }

        private void UpdateAuthTriesCounters()
        {
            long successfulTriesCount = (from t in App.DB.users_auth_tries
                                         where t.is_success == true
                                         select new { }).Count();

            long unsuccessfulTriesCount = (from t in App.DB.users_auth_tries
                                           where t.is_success == false
                                           select new { }).Count();

            SuccessfulAuthTriesCountTextBox.Text = successfulTriesCount.ToString();
            UnsuccessfulAuthTriesCountTextBox.Text = unsuccessfulTriesCount.ToString();
            AllAuthTriesCountTextBox.Text = (successfulTriesCount + unsuccessfulTriesCount).ToString();
        }

        private void TriedAtDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var query = (from t in App.DB.users_auth_tries
                         select t);
            DatePicker picker = sender as DatePicker;
            if (picker.SelectedDate.HasValue)
            {
                query = query.Where(t => DbFunctions.DiffDays(t.tried_at, picker.SelectedDate.Value) == 0);
            }
            
            if (CurrentUser.id != -1)
            {
                query = query.Where(t => t.user == CurrentUser.id);
            }

            paginationController.EntriesCountChanged(query.Count());
        }
    }
}
