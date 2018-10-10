using MaoriSouvenirShopping.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace MaoriSouvenirShopping.Models
{
    public class ShoppingCart
    {
        public string ShoppingCartID { get; set; }
        public const string CartSessionKey = "cartId";
        public static ShoppingCart GetCart(HttpContext context)
        {
            var cart = new ShoppingCart();
            cart.ShoppingCartID = cart.GetCartId(context);
            return cart;
        }
        public void AddToCart(Souvenir souvenir, ApplicationDbContext db)
        {
            var cartItem = db.CartItems.SingleOrDefault(c => c.CartID == ShoppingCartID && c.Souvenir.SouvenirID == souvenir.SouvenirID);
            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    Souvenir = souvenir,
                    CartID = ShoppingCartID,
                    Count = 1,
                    DateCreated = DateTime.Now
                };
                db.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Count++;
            }
            db.SaveChanges();
        }
        public Object UpdateModel(ApplicationDbContext db)
        {
            var CartItems = this.GetCartItems(db);
            var CartTotal = this.GetTotal(db);
            ArrayList Items = new ArrayList();
            foreach (CartItem c in CartItems)
            {
                Items.Add(new
                {
                    ID = c.Souvenir.SouvenirID,
                    Name = c.Souvenir.SouvenirName,
                    Price = c.Souvenir.Price,
                    Quantity = c.Count
                });
            }
            var model = new
            {
                Total = CartTotal,
                GST = CartTotal * (decimal)0.15,
                Sub = CartTotal * (decimal)0.85,
                Items = Items
            };
            return model;
        }
        public Object RemoveFromCart(int id, ApplicationDbContext db)
        {
            var cartItem = db.CartItems.SingleOrDefault(cart => cart.CartID == ShoppingCartID && cart.Souvenir.SouvenirID == id);
            int itemCount = 0;
            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                    itemCount = cartItem.Count;
                }
                else
                {
                    db.CartItems.Remove(cartItem);
                }
                db.SaveChanges();
            }
            return UpdateModel(db);
        }
        public Object EmptyCart(ApplicationDbContext db)
        {
            var cartItems = db.CartItems.Where(cart => cart.CartID == ShoppingCartID);
            foreach (var cartItem in cartItems)
            {
                db.CartItems.Remove(cartItem);
            }
            db.SaveChanges();
            return UpdateModel(db);
        }
        public List<CartItem> GetCartItems(ApplicationDbContext db)
        {
            List<CartItem> cartItems = db.CartItems.Include(i => i.Souvenir).ThenInclude(s => s.Category).Where(cartItem => cartItem.CartID == ShoppingCartID).ToList();

            return cartItems;

        }
        public int GetCount(ApplicationDbContext db)
        {
            int? count =
                (from cartItems in db.CartItems where cartItems.CartID == ShoppingCartID select (int?)cartItems.Count).Sum();
            return count ?? 0;
        }
        public decimal GetTotal(ApplicationDbContext db)
        {
            decimal? total = (from cartItems in db.CartItems
                              where cartItems.CartID == ShoppingCartID
                              select (int?)cartItems.Count * cartItems.Souvenir.Price).Sum();
            return total ?? decimal.Zero;
        }
        public string GetCartId(HttpContext context)
        {
            if (context.Session.GetString(CartSessionKey) == null)
            {
                Guid tempCartId = Guid.NewGuid();
                context.Session.SetString(CartSessionKey, tempCartId.ToString());
            }
            return context.Session.GetString(CartSessionKey).ToString();
        }

    }
}
