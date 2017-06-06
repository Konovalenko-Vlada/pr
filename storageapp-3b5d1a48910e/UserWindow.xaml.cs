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
using System.Windows.Shapes;
using System.Diagnostics;

namespace Storage
{
    /// <summary>
    /// Логика взаимодействия для UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        public User CurrentUser { get; private set; }

        public UserWindow(User TransferedUser)
        {
            InitializeComponent();

            CurrentUser = TransferedUser;
        }

        private void Window_Loaded(object sender, RoutedEventArgs ev)
        {
            dynamic CurrentView;

            switch(CurrentUser.Role.Flags)
            {
                case RoleFlags.Admin:
                    CurrentView = new AdminView(CurrentUser);
                    break;
                case RoleFlags.Shipper:
                    CurrentView = new ShipperView(CurrentUser);
                    break;
                default:
                    CurrentView = new UserView(CurrentUser);
                    break;
            }
            DockPanel.SetDock(CurrentView, Dock.Top);
            UserWindowDock.Children.Add(CurrentView);
        }

        private void UserLogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow.Instance.ProcessLogout();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            UserNameLabel.Content += CurrentUser.PublicName;
        }
    }
}
