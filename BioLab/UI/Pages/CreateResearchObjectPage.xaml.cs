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
        private long SelectedPatientId 
        { 
            get
            {
                 long? id = (patientsDataGrid.SelectedItem as PatientsShrinked).id;
                 if (id.HasValue)
                    {
                        return unchecked((long)id);
                    }
                return -1;
            }
        }
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
            long selectedPatientId = SelectedPatientId;
            if (selectedPatientId != -1)
            {
                patient selectedPatient;
                try
                {
                     selectedPatient = (from p in App.DB.patients
                                               where p.id == selectedPatientId
                                               select p).Single();
                }
                catch (InvalidOperationException exception)
                {
                    MessageBox.Show("Произошла ошибка при попытке получения данных о пациенте: " + exception.Message, "Ошибка приема пробирки", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                long barcode;

                if (!long.TryParse(barcodeField.Text, out barcode))
                {
                    MessageBox.Show("Указан недопустимый штри-код пробирки", "Ошибка приема пробирки", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                research_objects researchObject = new research_objects();
                researchObject.barcode = barcode;
                researchObject.date = DateTime.Now;
                researchObject.patient = selectedPatient.id;

                try
                {
                    App.DB.research_objects.Add(researchObject);
                    App.DB.SaveChanges();
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Произошла ошибка при попытке добавления записи в базу данных: " + exception.Message, "Ошибка приема пробирки", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                MessageBox.Show("Запись успешно добавлена!", "Прием пробирок", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Пациент должен быть обязательно выбран!", "Ошибка приема пробирки", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

