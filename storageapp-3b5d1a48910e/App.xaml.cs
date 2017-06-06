using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows;
using System.Data;
using System.Data.SqlClient;

namespace Storage
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs ev)
        {
            DatabaseDataContext DBC = new DatabaseDataContext();
            if (!DBC.DatabaseExists())
            {
                DBC.PrepareDatabase();
            }
            if (DBC.DatabaseExists())
            {
                Storage.LoginWindow.Instance.Show();
            }
            
        }
    }
}
