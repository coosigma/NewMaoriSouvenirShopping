using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MaoriSouvenirShopping.Models;

namespace MaoriSouvenirShopping.Data
{
    public class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();
            if (!context.Categories.Any())
            {
                var categories = new Category[]
                {
                    new Category{CategoryName="MaoriGift", Description="Maori Gifts"},
                    new Category{CategoryName="Jewel", Description="Jewels"},
                    new Category{CategoryName="Craft", Description="Crafts"},
                    new Category{CategoryName="Art", Description="Arts"},
                    new Category{CategoryName="Food", Description="Foods"}
                };
                foreach (Category c in categories)
                {
                    context.Categories.Add(c);
                }
                context.SaveChanges();
            }
            if (!context.Suppliers.Any())
            {
                var suppliers = new Supplier[]
                {
                    new Supplier{FirstName="Amada", LastName="Kia", Description="Maori Gifts Sup.", PhoneNumber="0211234567", Email="ak@email.com", Address="131 ABC"},
                    new Supplier{FirstName="John", LastName="Rich", Description="Jewels Sup.", PhoneNumber="0211234566", Email="jr@email.com", Address="132 ABC"},
                    new Supplier{FirstName="Mary", LastName="Well", Description="Crafts Sup.", PhoneNumber="0211234565", Email="mw@email.com", Address="133 ABC"},
                    new Supplier{FirstName="Tom", LastName="Smart", Description="Arts Sup.", PhoneNumber="0211234564", Email="ts@email.com", Address="134 ABC"},
                    new Supplier{FirstName="Jack", LastName="Big", Description="Foods Sup.", PhoneNumber="0211234563", Email="jb@email.com", Address="135 ABC"},
                };
                foreach (Supplier s in suppliers)
                {
                    context.Suppliers.Add(s);
                }
                context.SaveChanges();
            }
            if (!context.Souvenirs.Any())
            {
                var souvenirs = new Souvenir[]
                {
                    new Souvenir{SouvenirName="SurvivalKit", Price=(decimal)24.95, Description="Survival Kit", CategoryID=1, SupplierID=1, PhotoPath=""},
                    new Souvenir{SouvenirName="DollsKit", Price=(decimal)35.45, Description="Dolls kit", CategoryID=1, SupplierID=1, PhotoPath=""},
                    new Souvenir{SouvenirName="Jade0", Price=(decimal)500, Description="J0", CategoryID=2, SupplierID=2, PhotoPath=""},
                    new Souvenir{SouvenirName="Jade1", Price=(decimal)100, Description="J1", CategoryID=2, SupplierID=2, PhotoPath=""},
                    new Souvenir{SouvenirName="Jade2", Price=(decimal)200, Description="J2", CategoryID=2, SupplierID=2, PhotoPath=""},
                    new Souvenir{SouvenirName="Jade3", Price=(decimal)300, Description="J3", CategoryID=2, SupplierID=2, PhotoPath=""},
                    new Souvenir{SouvenirName="Jade4", Price=(decimal)400, Description="J4", CategoryID=2, SupplierID=2, PhotoPath=""},
                    new Souvenir{SouvenirName="BoatCraft", Price=(decimal)100, Description="Boat Craft", CategoryID=3, SupplierID=3, PhotoPath=""},
                    new Souvenir{SouvenirName="Postcard", Price=(decimal)1, Description="Postcard", CategoryID=4, SupplierID=4, PhotoPath=""},
                    new Souvenir{SouvenirName="Kai", Price=(decimal)30, Description="Sea food", CategoryID=5, SupplierID=5, PhotoPath=""},
                };
                foreach (Souvenir s in souvenirs)
                {
                    context.Souvenirs.Add(s);
                }
                context.SaveChanges();
            }

        }
    }
}
