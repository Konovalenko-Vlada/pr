using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Storage
{
    /*
     *  Пока-что роли разибты примитивно:
     *  К каждому флагу должно быть привязан конкретное действие
     *  (Взомжоность доабвления товара, заполнения анкеты, и.т.д.)
     */
    public enum RoleFlags : int {
        Admin = 0x00,
        User = 0x01,
        Shipper = 0x30
    }

    public enum OrderStatus : int {
        Prepared,
        Sended
    }
}