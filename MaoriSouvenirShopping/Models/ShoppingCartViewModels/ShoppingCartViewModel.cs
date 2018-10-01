using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaoriSouvenirShopping.Models.ShoppingCartViewModels
{
    public class ShoppingCartViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public decimal CartTotal { get; set; }
        public decimal TotalGST => Math.Round(CartTotal * (decimal)0.15, 2);
        public decimal SubTotal => Math.Round(CartTotal - TotalGST, 2);
    }
}
