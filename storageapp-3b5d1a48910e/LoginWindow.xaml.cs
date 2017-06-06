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
using System.Security.Cryptography;
using System.Collections;

namespace Storage
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public static LoginWindow Instance { get; private set; }
        private UserWindow CurrentView;

        static LoginWindow()
        {
            Instance = new LoginWindow();
        }

        public LoginWindow()
        {
            InitializeComponent();
        }

        public void ProcessLogout()
        {
            CurrentView.Hide();
            CurrentView = default(UserWindow);
            Instance.Show();
        }

        private void StoreLoginData()
        {
            Properties.Settings.Default.SavedUsername = LoginName.Text;
            Properties.Settings.Default.SavedPassword = LoginPassword.Password;
            Properties.Settings.Default.Save();
        }

        private bool IsLoginDataStored()
        {
            return (Properties.Settings.Default.SavedUsername != null &&
                    Properties.Settings.Default.SavedPassword != null);
        }

        private void RestoreLoginData()
        {
            LoginName.Text = Properties.Settings.Default.SavedUsername;
            LoginPassword.Password = Properties.Settings.Default.SavedPassword;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs ev)
        {
            DatabaseDataContext DBC = new DatabaseDataContext();
            String Username = LoginName.Text;
            MD5 MD5Handler = MD5.Create();
            MD5Handler.ComputeHash(Encoding.Default.GetBytes(LoginPassword.Password));
            byte[] Password = MD5Handler.Hash;

            var query = (from c in DBC.GetTable<User>()
                         where c.Login == Username
                         select c);
            if (query.Count() == 0)
            {
                MessageBox.Show(
                    "Пользователя с таким именем не существует", "Вход",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            User DBUser = query.Single();
            if (!StructuralComparisons.StructuralEqualityComparer.Equals(DBUser.Password, Password))
            {
                MessageBox.Show(
                    "Введенный пароль неверен! \r\n" +
                    "Проверьте правильность ввода пароля и повторите попытку",
                    "Вход", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(IsStoreLoginData.IsChecked ?? true)
            {
                StoreLoginData();
            }

            Instance.Hide();
            CurrentView = new UserWindow(DBUser);
            CurrentView.Show();
        }

        private void GotoRegister_Click(object sender, RoutedEventArgs ev)
        {
            Storage.RegisterWindow.Instance.Show();
            Instance.Hide();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(IsLoginDataStored())
            {
                RestoreLoginData();
            }
        }
    }
}
