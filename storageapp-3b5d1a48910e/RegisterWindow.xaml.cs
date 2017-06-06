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

namespace Storage
{
    /// <summary>
    /// Логика взаимодействия для RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public const int PreferredPasswordLength = 7;
        public static RegisterWindow Instance { get; private set; }

        static RegisterWindow()
        {
            Instance = new RegisterWindow();
        }

        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void ShowLoginForm()
        {
            Storage.LoginWindow.Instance.Show();
            Instance.Hide();
        }

        private void GotoLogin_Click(object sender, RoutedEventArgs e)
        {
            ShowLoginForm();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            DatabaseDataContext DBC = new DatabaseDataContext();
            MD5 MD5Handler = MD5.Create();
            String Login = RegisterName.Text;
            byte[] Password;

            if (RegisterPassword.Password.Length < PreferredPasswordLength)
            {
                MessageBox.Show(
                    "Пароль сликом короткий! Предпочтительная длинна: " + PreferredPasswordLength + " символов.",
                    "Регистрация", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!RegisterPassword.Password.Equals(RegisterPasswordRetype.Password))
            {
                MessageBox.Show(
                    "Пароли не совпадают!", "Регистрация",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MD5Handler.ComputeHash(Encoding.Default.GetBytes(RegisterPassword.Password));
            Password = MD5Handler.Hash;

            IQueryable<User> query = (from c in DBC.GetTable<User>()
                                      where c.Login == Login
                                      select c);
            if(query.Count() != 0)
            {
                MessageBox.Show(
                    "Пользователь с таким именем уже существует", "Регистрация",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Role UserRole = (from c in DBC.GetTable<Role>()
                             where c.Flags == RoleFlags.User
                             select c).SingleOrDefault();

            User NewUser = new User();
            NewUser.Login = Login;
            NewUser.Password = Password;
            NewUser.Role = UserRole;
            DBC.User.InsertOnSubmit(NewUser);

            Meta NewUserMeta = new Meta();
            NewUserMeta.Address = "";
            NewUserMeta.Name = "";
            NewUser.Meta = NewUserMeta;
            DBC.Meta.InsertOnSubmit(NewUserMeta);

           
            DBC.SubmitChanges();

            ShowLoginForm();
            
        }
    }
}
