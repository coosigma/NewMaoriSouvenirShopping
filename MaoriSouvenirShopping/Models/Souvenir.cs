using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MaoriSouvenirShopping.Models
{
    public class Souvenir
    {
        public int SouvenirID { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string SouvenirName { get; set; }
        [Required]
        public decimal Price { get; set; }
        public string PhotoPath { get; set; }
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Description { get; set; }
        [Required]
        public int CategoryID { get; set; }
        [Required]
        public int SupplierID { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }

        public Category Category { get; set; }
        public Supplier Supplier { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }
}
