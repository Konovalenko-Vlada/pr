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

namespace Storage
{
    /// <summary>
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        DatabaseDataContext DBC = new DatabaseDataContext();
        List<ProductList> OrderedProducts = new List<Storage.ProductList>();
        Order CurrentOrder;

        public OrderWindow(IEnumerable<Product> ProductList, User Purchaser, User Shipper)
        {
            InitializeComponent();

            CurrentOrder = new Order()
            {
                Purchaser = (from c in DBC.User where c.Id == Purchaser.Id select c).Single(),
                Shipper = (from c in DBC.User where c.Id == Shipper.Id select c).Single(),
                Date = DateTime.Now,
                TotalPrice = 0
            };
            DBC.Order.InsertOnSubmit(CurrentOrder);

            foreach (Product Product in ProductList)
            {
                OrderedProducts.Add(new ProductList(){
                    Product = (from c in DBC.Product where c.Id == Product.Id select c).Single(),
                    Order = CurrentOrder,
                    Count = Product.BuyCount
                });
                CurrentOrder.TotalPrice += (Product.Price * Product.BuyCount);
            }
            DBC.ProductList.InsertAllOnSubmit(OrderedProducts);
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            PaymentPurchaser.Text = CurrentOrder.Purchaser.PublicName;
            PaymentShipper.Text = CurrentOrder.Shipper.PublicName;
            PaymentTotalPrice.Text = CurrentOrder.TotalPrice.ToString();
        }

        private void PayButton_Click(object sender, RoutedEventArgs e)
        {
            DBC.SubmitChanges();
            this.Close();
        }
    }
}
