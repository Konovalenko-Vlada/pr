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

namespace Storage
{
    /// <summary>
    /// Логика взаимодействия для ShipperView.xaml
    /// </summary>
    public partial class ShipperView : UserControl
    {
        DatabaseDataContext DBC = new DatabaseDataContext();
        public User CurrentUser;

        public ShipperView(User TransferedUser)
        {
            InitializeComponent();

            CurrentUser = (
                from c in DBC.User
                where c.Id == TransferedUser.Id
                select c).Single();
        }

        private void LoadShipment()
        {
            var Query = (from c in DBC.Product
                         where c.Shipper == CurrentUser
                         select c);
            ShipperGrid.ItemsSource = Query;
        }

        private void DisableEditControls()
        {
            ApplyShipmentButton.IsEnabled = false;
            ResetShipmentButton.IsEnabled = false;
        }

        private void EnableEditControls()
        {
            ApplyShipmentButton.IsEnabled = true;
            ResetShipmentButton.IsEnabled = true;
        }

        private void LoadProfile()
        {
            ProfileName.Text = CurrentUser.Meta.Name;
            ProfileAddress.Text = CurrentUser.Meta.Address;
        }

        private void UpdateOrders()
        {
            OrdersGrid.ItemsSource = (
                from c in DBC.Order
                where c.Shipper == CurrentUser
                select c);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadShipment();
            LoadProfile();
            UpdateOrders();
        }

        private void ApplyShipmentButton_Click(object sender, RoutedEventArgs ev)
        {
            MessageBoxResult result = MessageBox.Show(
                "Вы действительно хотите применить изменения?", "Редактирование", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes)
            {
                DBC.SubmitChanges();
                DisableEditControls();
            }
        }

        private void ResetShipmentButton_Click(object sender, RoutedEventArgs e)
        {
            LoadShipment();
        }

        private void ShipperGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            Product EditingItem = ShipperGrid.SelectedValue as Product;

            if(EditingItem.Shipper_id == 0)
            {
                EditingItem.Shipper_id = CurrentUser.Id;
            }

            EnableEditControls();
        }

        private void ShipperGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            EnableEditControls();
        }

        private void ApplyProfileButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser.Meta.Name = ProfileName.Text;
            CurrentUser.Meta.Address = ProfileAddress.Text;
            DBC.SubmitChanges();
        }

        private void UpdateOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateOrders();
        }

        private void MarkForSend_Click(object sender, RoutedEventArgs e)
        {
            Order CurrentOrder = OrdersGrid.SelectedItem as Order;
            CurrentOrder.Status = OrderStatus.Sended;
            CurrentOrder.CalculateProductCounts();

            UpdateOrders();
        }
    }
}
