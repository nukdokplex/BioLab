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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BioLab.UI.Pages
{
    /// <summary>
    /// Логика взаимодействия для CreateOrderPage.xaml
    /// </summary>
    public partial class CreateOrderPage : Page
    {
        private PaginationController PatientsPaginationController, ResearchObjectsPaginationController, ServicesPaginationController;
        private PaginationControls PatientsPaginationControls, ResearchObjectsPaginationControls, ServicesPaginationControls;
        private patient CurrentPatient = new patient();

        private void PatientNameSearchField_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox textBox = sender as TextBox;
                if (!string.IsNullOrWhiteSpace(textBox.Text))
                {
                    long count = (from p in App.DB.patients
                                             where p.full_name.Contains(textBox.Text)
                                             orderby p.id ascending
                                             select new object()).Count();
                    PatientsPaginationController.CurrentSearchQuery = textBox.Text;
                    PatientsPaginationController.EntriesCountChanged(count);
                }
                else
                {
                    PatientsPaginationController.CurrentSearchQuery = string.Empty;
                    PatientsPaginationController.EntriesCountChanged(App.DB.patients.Count());
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow.header.HeaderTitle.Text = this.Title;
            InitializePagination();
        }

        private void SelectPatientButton_Click(object sender, RoutedEventArgs e)
        {
            long? selectedPatientId = (patientsDataGrid.SelectedItem as PatientsShrinked).id;
            if (selectedPatientId.HasValue)
            {
                CurrentPatient = (from p in App.DB.patients
                                  where p.id == selectedPatientId.Value
                                  select p).Single();
                SelectedPatientTextBox.Text = CurrentPatient.id.ToString() + " - " + CurrentPatient.full_name;
                PatientsUISetEnabledState(false);
                InitializeResearchObjectsPagination();
            }
            else
            {
                MessageBox.Show(
                    "Сначала выберите пацента в таблице",
                    "Ошибка!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void InitializeResearchObjectsPagination()
        {
            ResearchObjectsUISetEnabledState(true);
            long count = (from ro in App.DB.research_objects
                          where ro.patient == CurrentPatient.id
                          select new object()).Count();
            ResearchObjectsPaginationController = new PaginationController(20, count, ResearchObjectsPaginationControls);
            ResearchObjectsPaginationController.OnRefresh += ResearchObjectsRefresh;
            ResearchObjectsPaginationController.CurrentPage = 0;
        }

        private void PatientsUISetEnabledState(bool state)
        {
            PatientsPaginationControls.SetEnabledState(state);
            PatientNameSearchField.IsEnabled = state;
            patientsDataGrid.Columns.Clear();
            patientsDataGrid.IsEnabled = state;
            SelectPatientButton.IsEnabled = state;
        }

        private void ResearchObjectsUISetEnabledState(bool state)
        {
            ResearchObjectsPaginationControls.SetEnabledState(state);
            ResearchObjectsSearchField.IsEnabled = state;
            ResearchObjectsDataGrid.Columns.Clear();
            ResearchObjectsDataGrid.IsEnabled = state;
            SelectResearchObjectButton.IsEnabled = state;
        }

        public CreateOrderPage()
        {
            CurrentPatient = new patient();
            CurrentPatient.id = -1;
            InitializeComponent();
            
        }

        private void ServicesSearchField_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox textBox = sender as TextBox;
                if (!string.IsNullOrWhiteSpace(textBox.Text))
                {
                    List<object> services = (from s in App.DB.services
                                             where App.currentUserServices.Contains(s.id) &&
                                             s.name.Contains(textBox.Text)
                                             select new object()).ToList();
                    ServicesPaginationController.CurrentSearchQuery = textBox.Text;
                    ServicesPaginationController.EntriesCountChanged(services.Count());

                }
                else
                {
                    List<object> services = (from s in App.DB.services
                                             where App.currentUserServices.Contains(s.id)
                                             select new object()).ToList();
                    ServicesPaginationController.CurrentSearchQuery = string.Empty;
                    ServicesPaginationController.EntriesCountChanged(services.Count());
                }
            }
        }

        private List<long> GetSelectedServices()
        {
            List<long> services = new List<long>();
            foreach (UIElement s in SelectedServicesContainer.Children)
            {
                services.Add(((s as StackPanel).Tag as long?).Value);
            }
            return services;
        }

        private List<long> GetSelectedResearchObjects()
        {
            List<long> ros = new List<long>();
            foreach (UIElement ro in SelectedResearchObjectsContainer.Children)
            {
                ros.Add(((ro as StackPanel).Tag as long?).Value);
            }
            return ros;
        }

        private void SelectServiceButton_Click(object sender, RoutedEventArgs e)
        {
            long? selectedServiceId = (ServicesDataGrid.SelectedItem as service).id;
            if (selectedServiceId.HasValue)
            {
                service s = (from serv in App.DB.services
                             where serv.id == selectedServiceId.Value
                             select serv).Single();
                StackPanel serviceUIElement = new StackPanel();
                serviceUIElement.Orientation = Orientation.Horizontal;
                TextBlock serviceName = new TextBlock();
                serviceName.Text = s.name;
                Button removeServiceButton = new Button();
                removeServiceButton.Content = "[x]";
                removeServiceButton.Click += DeleteService;
                serviceUIElement.Tag = selectedServiceId.Value;
                serviceUIElement.Children.Add(serviceName);
                serviceUIElement.Children.Add(removeServiceButton);
                SelectedServicesContainer.Children.Add(serviceUIElement);
            }
            else
            {
                MessageBox.Show(
                    "Сначала выберите услугу в таблице",
                    "Ошибка!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void DeleteService(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            StackPanel parent = button.Parent as StackPanel;
            (parent.Parent as StackPanel).Children.Remove(parent);
        }

        private void SelectResearchObjectButton_Click(object sender, RoutedEventArgs e)
        {
            long? selectedResearchObjectId = (ResearchObjectsDataGrid.SelectedItem as research_objects).id;
            if (selectedResearchObjectId.HasValue)
            {
                research_objects ro = (from rob in App.DB.research_objects
                                       where rob.id == selectedResearchObjectId.Value
                                       select rob).Single();
                StackPanel researchObjectUIElement = new StackPanel();
                researchObjectUIElement.Orientation = Orientation.Horizontal;
                TextBlock researchObjectName = new TextBlock();
                researchObjectName.Text = ro.id.ToString();
                Button removeResearchObjectButton = new Button();
                removeResearchObjectButton.Content = "[x]";
                removeResearchObjectButton.Click += DeleteService;
                researchObjectUIElement.Tag = ro.id;
                researchObjectUIElement.Children.Add(researchObjectName);
                researchObjectUIElement.Children.Add(removeResearchObjectButton);
                SelectedResearchObjectsContainer.Children.Add(researchObjectUIElement);
            }
            else
            {
                MessageBox.Show(
                    "Сначала выберите пробирку в таблице",
                    "Ошибка!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void CreateOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPatient.id == -1)
            {
                MessageBox.Show(
                    "Вы должны выбрать пациента!", 
                    "Ошибка!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            List<long> selectedResearchObjectIds = GetSelectedResearchObjects();
            
            if (selectedResearchObjectIds.Count() < 1)
            {
                MessageBox.Show(
                    "Вы должны выбрать хотя бы одну пробирку!",
                    "Ошибка!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            List<long> selectedServicesIds = GetSelectedServices();

            if (selectedServicesIds.Count() < 1)
            {
                MessageBox.Show(
                    "Вы должны выбрать хотя бы одну услугу!",
                    "Ошибка!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            order o = new order();
            o.user = App.currentUser.id;
            o.status = 1;

            o = App.DB.orders.Add(o);
            App.DB.SaveChanges();

            List<order_research_objects> oros = new List<order_research_objects>();
            foreach(long roId in selectedResearchObjectIds)
            {
                order_research_objects oro = new order_research_objects();
                oro.order = o.id;
                oro.research_object = roId;
            }

            List<order_services> oss = new List<order_services>();
            foreach (long sId in selectedServicesIds)
            {
                order_services os = new order_services();
                os.order = o.id;
                os.service = sId; 
            }

            App.DB.order_research_objects.AddRange(oros);
            App.DB.order_services.AddRange(oss);

            App.DB.SaveChanges();
            MessageBox.Show(
                "Заказ успешно добавлен!",
                "Успех",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void InitializePagination() {
            PatientsPaginationControls = new PaginationControls(PatientsFirstPageButton, PatientsPreviousPageButton, PatientsNextPage, PatientsLastPage, PatientsCurrentPageTextBox);
            ResearchObjectsPaginationControls = new PaginationControls(ResearchObjectsFirstPageButton, ResearchObjectsPreviousPageButton, ResearchObjectsNextPage, ResearchObjectsLastPage, ResearchObjectsCurrentPageTextBox);
            ServicesPaginationControls = new PaginationControls(ServicesFirstPageButton, ServicesPreviousPageButton, ServicesNextPage, ServicesLastPage, ServicesCurrentPageTextBox);

            long patientsCount = App.DB.patients.Count();
            long servicesCount = App.currentUserServices.Count();

            ServicesPaginationController = new PaginationController(20, servicesCount, ServicesPaginationControls);
            ServicesPaginationController.OnRefresh += ServicesRefresh;
            ServicesPaginationController.CurrentPage = 0;

            PatientsPaginationController = new PaginationController(20, patientsCount, PatientsPaginationControls);
            PatientsPaginationController.OnRefresh += PatientsRefresh;
            PatientsPaginationController.CurrentPage = 0;

            // ResearchOjects Pagination will be initialized on patient selected
        }

        private void PatientsRefresh(object sender, PaginationOnRefreshEventArgs e)
        {
            PaginationController controller = sender as PaginationController;
            List<PatientsShrinked> patients;
            if (!string.IsNullOrWhiteSpace(e.SearchQuery))
            {
                patients = (from p in App.DB.patients
                            where p.full_name.Contains(e.SearchQuery)
                            orderby p.id ascending
                            select new PatientsShrinked
                            {
                                id = p.id,
                                full_name = p.full_name,
                                birthdate = p.birthdate,
                                passport_number = p.passport_number,
                                passport_serial = p.passport_serial
                            }).Skip(unchecked((int)e.CurrentOffset)).Take(20).ToList();
            }
            else
            {
                patients = (from p in App.DB.patients
                            orderby p.id ascending
                            select new PatientsShrinked
                            {
                                id = p.id,
                                full_name = p.full_name,
                                birthdate = p.birthdate,
                                passport_number = p.passport_number,
                                passport_serial = p.passport_serial
                            }).Skip(unchecked((int)e.CurrentOffset)).Take(20).ToList();
            }
            patientsDataGrid.Columns.Clear();
            patientsDataGrid.ItemsSource = patients;
            e.Controls.SetEnabledState(true);
        }

        private void ResearchObjectsRefresh(object sender, PaginationOnRefreshEventArgs e)
        {
            List<research_objects> ros = new List<research_objects>();
            long barcode = 0;
            if (!string.IsNullOrWhiteSpace(ResearchObjectsSearchField.Text) && long.TryParse(ResearchObjectsSearchField.Text, out barcode))
            {
                ros = (from ro in App.DB.research_objects
                       where ro.patient == CurrentPatient.id &&
                       ro.barcode == barcode
                       orderby ro.id ascending
                       select ro).Skip(unchecked((int)e.CurrentOffset)).Take(20).ToList();
            }
            else
            {
                ros = (from ro in App.DB.research_objects
                       where ro.patient == CurrentPatient.id
                       orderby ro.id ascending
                       select ro).Skip(unchecked((int)e.CurrentOffset)).Take(20).ToList();
            }

            ResearchObjectsDataGrid.Columns.Clear();
            ResearchObjectsDataGrid.ItemsSource = ros;

            e.Controls.SetEnabledState(true);
        }

        private void ServicesRefresh(object sender, PaginationOnRefreshEventArgs e)
        {
            List<service> services = new List<service>();
            if (!string.IsNullOrWhiteSpace(ServicesSearchField.Text))
            {
                services = (from s in App.DB.services
                            where App.currentUserServices.Contains(s.id) &&
                            s.name.Contains(ServicesSearchField.Text)
                            orderby s.id ascending
                            select s)
                            .Skip(unchecked((int)e.CurrentOffset)).Take(20).ToList();
            }
            else
            {
                services = (from s in App.DB.services
                            where App.currentUserServices.Contains(s.id)
                            orderby s.id ascending
                            select s)
                            .Skip(unchecked((int)e.CurrentOffset)).Take(20).ToList();
            }

            ServicesDataGrid.Columns.Clear();
            ServicesDataGrid.ItemsSource = services;

            e.Controls.SetEnabledState(true);
        }
    }
}
