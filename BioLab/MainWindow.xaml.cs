using BioLab.UI.Elements;
using BioLab.UI.Pages;
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


namespace BioLab
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Header header;
        
        public MainWindow()
        {
            InitializeComponent();
            App.MainWindow = this;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BodyContainer.Navigate(new AuthorizationPage());
            header = new Header();
            HeaderContainer.Navigate(header);

        }
    }
}
