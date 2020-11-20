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
        public StartPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
            MainWindow.header.HeaderTitle.Text = Title;
            userName.Text = App.currentUser.name;
            UserImage.Source = new BitmapImage(new Uri(@"\Resources\Images\laborant_"+App.currentUser.user_type.ToString()+".jpeg", UriKind.RelativeOrAbsolute));
            userType.Text = (from user_type in App.DB.user_types
                        where user_type.id == App.currentUser.user_type
                        select user_type).Single().name;
            List<long> services = new List<long>();
            if (App.currentUser.services == String.Empty)
            {
                App.currentUser.services = "[]";
            }
            List<BioLab.Database.DataModels.Service> jsonServices = 
                Newtonsoft.Json.JsonConvert.DeserializeObject<List<BioLab.Database.DataModels.Service>>(App.currentUser.services);
            foreach (BioLab.Database.DataModels.Service service in jsonServices)
            {
                services.Add(service.code);
            }
            List<string> serviceNames = new List<string>();
            serviceNames = (from service in App.DB.services
                            where services.Contains(service.id)
                            select service.name).ToList();
            UserServices.Children.Clear();
            foreach (string service in serviceNames)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = service;
                textBlock.FontSize = 18;
                UserServices.Children.Add(textBlock);
            }
        }
    }
}
