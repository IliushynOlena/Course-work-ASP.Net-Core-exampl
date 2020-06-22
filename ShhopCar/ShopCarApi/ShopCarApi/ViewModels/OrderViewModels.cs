using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopCarApi.ViewModels
{
    public class OrderVM
    {
        public string Client { get; set; }
        public string Car { get; set; }
    }

    public class OrderAddVM
    {
        public ClientVM Client { get; set; }
        public CarVM Car { get; set; }
        public DateTime Date { get; set; }

    }
}
