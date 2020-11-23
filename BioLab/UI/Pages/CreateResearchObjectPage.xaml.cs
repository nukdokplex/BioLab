using BioLab.Database;
using BioLab.Database.DataModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BioLab.UI.Pages
{
    /// <summary>
    /// Логика взаимодействия для CreateResearchObjectPage.xaml
    /// </summary>
    public partial class CreateResearchObjectPage : Page
    {
        protected long _currentPage;
        private long CurrentPage {

            get
            {
                return _currentPage;
            }
            set
            {
                PaginationControlsToggleEnabledState(false);
                if (value <= LastPage && value >= 0)
                {
                    _currentPage = value;
                    CurrentPageTextBox.Text = (value + 1).ToString();
                    RefreshPatients();
                }
                PaginationControlsToggleEnabledState(true);
            }
        }
        private long LastPage = 0;
        private int EntriesPerPage = 20;
        private string CurrentSearchQuery = "";
        private int CurrentOffset {
            get
            {
                return unchecked((int)((CurrentPage) * EntriesPerPage));
            }
        }

        public CreateResearchObjectPage()
        {
            InitializeComponent();
        }

        private void PaginationControlsToggleEnabledState(bool isEnabled)
        {
            PaginationControlsContainer.IsEnabled = isEnabled;
        }



        private void CalculateLastPage(out long LastPage, long EntriesCount, int EntriesPerPage)
        {

            LastPage = (EntriesCount - 1) / EntriesPerPage + 1;
            LastPage--;
        }

        private void RefreshPatients()
        {
            List<PatientsShrinked> patients = new List<PatientsShrinked>();
            if (CurrentSearchQuery == string.Empty)
            {
                patients = (from p in App.DB.patients
                            orderby p.id ascending
                            select new PatientsShrinked 
                            { 
                                id = p.id, 
                                full_name = p.full_name, 
                                birthdate = p.birthdate, 
                                passport_serial = p.passport_serial, 
                                passport_number = p.passport_number
                            }
                            )
                            .Skip(CurrentOffset).Take(EntriesPerPage).ToList();


            }
            else
            {
                patients = (from p in App.DB.patients
                            where p.full_name.Contains(CurrentSearchQuery)
                            orderby p.id ascending
                            select new PatientsShrinked
                            {
                                id = p.id,
                                full_name = p.full_name,
                                birthdate = p.birthdate,
                                passport_serial = p.passport_serial,
                                passport_number = p.passport_number
                            }
                            )
                            .Skip(CurrentOffset).Take(EntriesPerPage).ToList();
            }
            patientsDataGrid.Columns.Clear();
            patientsDataGrid.ItemsSource = patients;
            PaginationControlsToggleEnabledState(true);
        }



        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            PaginationControlsToggleEnabledState(false);
            CalculateLastPage(out LastPage, App.DB.patients.Count(), EntriesPerPage);
            RefreshPatients();

        }

        private void PatientSearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(PatientNameSearchField.Text))
            {
                CurrentSearchQuery = PatientNameSearchField.Text;
                List<patient> patients = (from patient in App.DB.patients
                                          where DbFunctions.Like(patient.full_name, "*" + CurrentSearchQuery + "*")
                                          select patient).ToList();
                CalculateLastPage(out LastPage, patients.Count, EntriesPerPage);
                CurrentPage = 0;
                patients.Clear();


                RefreshPatients();
            }
            else
            {
                CurrentSearchQuery = string.Empty;
                CalculateLastPage(out LastPage, App.DB.patients.Count(), EntriesPerPage);
                CurrentPage = 0;


                RefreshPatients();
            }
        }

        private void CreateResearchObjectButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CurrentPageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            long currentPage;
            if (long.TryParse(CurrentPageTextBox.Text, out currentPage))
            {
                CurrentPage = currentPage - 1;
            }

        }

        private void PatientsFirstPageButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage = 0;
        }

        private void PatientsPreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage = CurrentPage - 1; // I dont think the ++/-- operator can deal with SET. Does it can?
        }

        private void PatientsNextPage_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage = CurrentPage + 1;
        }

        private void PatientsLastPage_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage = LastPage;
        }
    }
    
}

