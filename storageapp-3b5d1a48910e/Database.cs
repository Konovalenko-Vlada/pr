using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System;
using System.Windows;

namespace Storage
{
    partial class DatabaseDataContext
    {

        public void PrepareDatabase()
        {
            CreateDatabase();

            List<Role> BaseRoles = new List<Role>
            {
                new Role { Name = "Администратор", Flags = RoleFlags.Admin },
                new Role { Name = "Пользователь", Flags = RoleFlags.User },
                new Role { Name = "Поставщик", Flags = RoleFlags.Shipper }
            };
            Role.InsertAllOnSubmit(BaseRoles);
            SubmitChanges();

            // Создадим администратора-пустышку для дальнешего управления
            // После занесения информации данного администратора необходимо удалить
            MD5 MD5Handler = MD5.Create();
            MD5Handler.ComputeHash(Encoding.Default.GetBytes("12345"));
            User.InsertOnSubmit(new User { 
                Login = "admin",
                Password = MD5Handler.Hash,
                Role = (from c in Role where c.Flags == RoleFlags.Admin select c).Single()
            });
            SubmitChanges();
        }
    }

    partial class Product 
    {
        public bool IsSelectedForBuy { get; set; }
        public double BuyCount { get; set; }
    }

    partial class Order
    {
        public Visibility SendButtonVisibility
        {
            get
            {
                if(this.Status == OrderStatus.Prepared)
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public void CalculateProductCounts()
        {
            DatabaseDataContext DBC = new DatabaseDataContext();
            IQueryable<ProductList> OrderedProducts = (
                from c in DBC.ProductList
                where c.Order_id == this.Id
                select c);

            foreach (ProductList OrderedProduct in OrderedProducts)
            {
                OrderedProduct.Product.Count -= OrderedProduct.Count;
            }
            DBC.SubmitChanges();
        }
    }

    partial class User
    {
        public String PublicName {
            get
            {
                if (this.Meta.Name != "")
                {
                    return this.Meta.Name;
                }
                return Login;
            }
        }
    }
}