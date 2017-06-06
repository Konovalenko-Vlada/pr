using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для UserControl.xaml
    /// </summary>
    public partial class UserView : UserControl
    {
        DatabaseDataContext DBC = new DatabaseDataContext();
        User CurrentUser;
        Role ShipperRole;

        public UserView(User TransferedUser)
        {
            InitializeComponent();

            CurrentUser = (
                from c in DBC.User
                where c.Id == TransferedUser.Id
                select c).Single();

            ShipperRole = (
                from c in DBC.Role
                where c.Flags == RoleFlags.Shipper
                select c).Single();

        }

        private void UpdateShippers()
        {
            ShippersList.ItemsSource = (
                from c in DBC.User
                where c.Role == ShipperRole
                select c);
        }

        public void UpdateOrders()
        {
            OrdersGrid.ItemsSource = (
                from c in DBC.Order
                where c.Purchaser == CurrentUser
                select c);
        }

        private void LoadProfile()
        {
            ProfileName.Text = CurrentUser.Meta.Name;
            ProfileAddress.Text = CurrentUser.Meta.Address;
        }

        private bool HaveSelectedForBuy ()
        {
            foreach (Product Product in ShipmentGrid.Items)
            {
                if (Product.IsSelectedForBuy == true)
                {
                    return true;
                }
            }

            return false;
        }

        private void FillShipmentGrid()
        {
            User CurrentShipper = ShippersList.SelectedItem as User;

            if (CurrentShipper == null)
            {
                return;
            }

            ShipmentGrid.ItemsSource = (
               from c in DBC.Product
               where c.Shipper == CurrentShipper
               select c);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateShippers();
            UpdateOrders();
            LoadProfile();
        }

        private void UpdateOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateOrders();
        }

        private void UpdateShipmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (HaveSelectedForBuy())
            {
                MessageBoxResult Result = MessageBox.Show(
                    "Вы действительно хотите обновить список поставщиков??\n" +
                    "Выбранные у этого поставщика товары будут сброшены!\n",
                    "Магазин", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (Result == MessageBoxResult.No)
                {
                    return;
                }
            }

            UpdateShippers();
            ShipmentGrid.ItemsSource = null;
        }

        private void ApplyProfileButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser.Meta.Name = ProfileName.Text;
            CurrentUser.Meta.Address = ProfileAddress.Text;
            DBC.SubmitChanges();
        }

        private void ShippersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(HaveSelectedForBuy())
            {
                MessageBoxResult Result = MessageBox.Show(
                    "Вы действительно хотите посмотреть товары другого поставщика?\n" +
                    "Выбранные у этого поставщика товары будут сброшены!\n",
                    "Магазин", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (Result == MessageBoxResult.No)
                {
                    return;
                }
            }

            FillShipmentGrid();
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            Button BuyButton = sender as Button;
            TextBox CountBox = (BuyButton.Parent as StackPanel).Children[1] as TextBox;
            Product CurrentProduct = ShipmentGrid.SelectedItem as Product;

            CurrentProduct.IsSelectedForBuy = true;

            if (CountBox.Visibility != Visibility.Visible)
            {
                CountBox.Visibility = Visibility.Visible;
                BuyButton.Visibility = Visibility.Collapsed;
            }
        }

        private void BuyCounter_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox BuyCounter = sender as TextBox;
            Product CurrentProduct = ShipmentGrid.SelectedItem as Product;

            if(BuyCounter.Text != "")
            {
                double InputValue = Convert.ToDouble(BuyCounter.Text);
                if(InputValue > CurrentProduct.Count)
                {
                    BuyCounter.Text = CurrentProduct.Count.ToString();
                    InputValue = CurrentProduct.Count;
                }
                CurrentProduct.BuyCount = InputValue;
            }
        }

        private void BuyCounter_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void BuyShipmentsButton_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<Product> SelectedProducts = (
                from Product c in ShipmentGrid.Items
                where c.IsSelectedForBuy == true
                select c);

            OrderWindow NewOrderWindow = new OrderWindow(
                SelectedProducts, CurrentUser, ShippersList.SelectedItem as User);
            NewOrderWindow.Show();
            NewOrderWindow.Closed += NewOrderWindow_Closed;
        }

        private void NewOrderWindow_Closed(object sender, EventArgs e)
        {
            FillShipmentGrid();
        }
    }
}
