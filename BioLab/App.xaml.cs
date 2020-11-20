using BioLab.Database;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BioLab
{
    public partial class App : Application
    {
        public static entities DB;
        public static user currentUser;
        public static MainWindow MainWindow;
        protected override void OnStartup(StartupEventArgs e)
        {

            DB = new entities(); //initializing database on app startup
            base.OnStartup(e);
        }
    }
}
