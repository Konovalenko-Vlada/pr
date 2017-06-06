using System;
using System.Collections.Generic;
using System.Data.Linq;
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

namespace Storage
{
    /// <summary>
    /// Логика взаимодействия для AdminControl.xaml
    /// </summary>
    public partial class AdminView : UserControl
    {

        DatabaseDataContext DBC = new DatabaseDataContext();
        User CurrentUser;

        public AdminView(User TransferedUser)
        {
            CurrentUser = TransferedUser;
            InitializeComponent();
        }

        private void LoadDataToView()
        {
            String TableName = (DatabaseSelector.SelectedItem as ComboBoxItem).Content.ToString();

            DataView.ItemsSource = (ITable)DBC.GetType().GetProperty(TableName).GetValue(DBC, null);
        }

        private void DatabaseSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadDataToView();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataToView();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            DBC.SubmitChanges();
        }
    }
}
