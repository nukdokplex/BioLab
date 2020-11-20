using BioLab.Database;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BioLab.UI.Pages
{
    /// <summary>
    /// Логика взаимодействия для CreateResearchObjectPage.xaml
    /// </summary>
    public partial class CreateResearchObjectPage : Page
    {
        private long CurrentPage = 1;
        private long LastPage = 0;
        private int EntriesPerPage = 20;
        private long CurrentEntriesCount = 0;
        private string CurrentSearchQuery = "";
        private int CurrentOffset { 
            get
            {
                return unchecked((int)((CurrentPage - 1) * EntriesPerPage));
            } 
        }
        
        public CreateResearchObjectPage()
        {
            InitializeComponent();
            PaginationControlsToggleEnabledState();
        }

        private void PaginationControlsToggleEnabledState()
        {
            PaginationControlsContainer.IsEnabled = !PaginationControlsContainer.IsEnabled;
        }

        private void CalculateLastPage(out long LastPage, long EntriesCount, int EntriesPerPage)
        {

            LastPage = (EntriesCount - 1) / EntriesPerPage + 1;
        }

        private void RefreshPatients()
        {
            List <patient> patients;
            if (CurrentSearchQuery == string.Empty)
            {
                patients = (from patient in App.DB.patients
                            select patient).Skip(CurrentOffset).Take(EntriesPerPage).ToList();
                
                
            }
            else
            {
                patients = (from patient in App.DB.patients
                            where DbFunctions.Like(patient.full_name, "*" + CurrentSearchQuery + "*")
                            select patient).Skip(CurrentOffset).Take(EntriesPerPage).ToList();
            }
            patientsDataGrid.Columns.Clear();
            patientsDataGrid.ItemsSource = patients;
        }

        

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshPatients();
            CalculateLastPage(out LastPage, App.DB.patients.Count(), EntriesPerPage);
            
        }

        private void PatientSearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(PatientNameSearchField.Text))
            {
                CurrentSearchQuery = PatientNameSearchField.Text;
                RefreshPatients();
            }
            else
            {
                MessageBox.Show("Поле поиска не должно быть пустым!", "Ошибка поиска", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void CreateResearchObjectButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CurrentPageTextBox_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void PatientsFirstPageButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PatientsPreviousPageButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PatientsNextPage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PatientsLastPage_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
