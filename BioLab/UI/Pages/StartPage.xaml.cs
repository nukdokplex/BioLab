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
    public partial class StartPage : Page
    {
        private static bool isAuthRemovedFromBackStack = false;
        public StartPage()
        {
            
            InitializeComponent();
            DisableDisallowedMenuEntries(App.currentUser.user_type);
        }

        private void DisableDisallowedMenuEntries(long user_type)
        {
            foreach (Button button in MenuContainer.Children)
            {
                button.IsEnabled = user_type == long.Parse(button.Tag.ToString()); 
                // Just disabling buttons if user type not equals their tags
            }
        }

        private void ProcessBackStack()
        {
            if (!isAuthRemovedFromBackStack)
                NavigationService.RemoveBackEntry();
            isAuthRemovedFromBackStack = true;
        }

        public void RefreshUserInfo()
        {
            userName.Text = App.currentUser.name; // User real name

            UserImage.Source = new BitmapImage(new Uri(@"\Resources\Images\laborant_" + 
                App.currentUser.user_type.ToString() + ".jpeg", UriKind.RelativeOrAbsolute)); 
            // User avatars (images are already in project)

            userType.Text = (from user_type in App.DB.user_types
                             where user_type.id == App.currentUser.user_type
                             select user_type).Single().name; // Getting user type name

            List<long> services = new List<long>(); //Warm-up for services json parsing
            if (App.currentUser.services == String.Empty)
            {
                App.currentUser.services = "[]"; // Protecting from empty string (JSON converter can't deal with it)
            }

            List<BioLab.Database.DataModels.Service> jsonServices = 
                Newtonsoft.Json.JsonConvert
                .DeserializeObject<List<BioLab.Database.DataModels.Service>>(
                    App.currentUser.services
                    ); // Converting JSON to POJO

            foreach (BioLab.Database.DataModels.Service service in jsonServices)
            {
                services.Add(service.code); // Retrieving services codes from POJO
            }

            List<string> serviceNames = new List<string>();
            serviceNames = (from service in App.DB.services
                            where services.Contains(service.id)
                            select service.name).ToList(); // Getting service names by their codes (ids)
            UserServices.Children.Clear(); // Clearing StackPanel from previous service names
            foreach (string service in serviceNames)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = service;
                textBlock.FontSize = 18;
                UserServices.Children.Add(textBlock); // Adding service names (TextBlocks) to StackPanel (Container)
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ProcessBackStack(); //Deletes AuthForm from Navigation backstack

            MainWindow.header.HeaderTitle.Text = Title; //Setting page title in header

            RefreshUserInfo(); //Update user info in UI (not async, takes UI runtime)
        }

        private void CreateResearchObjectButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CreateResearchObjectPage());
        }
    }
}
