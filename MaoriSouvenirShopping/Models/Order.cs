using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MaoriSouvenirShopping.Models
{
    public enum Status
    {
        Waiting, Ordered, Billed, Paid, Shipped, Cancelled
    }
    public class Order
    {
        public int OrderID { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string LastName { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string City { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string State { get; set; }
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
        public string PostalCode { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Country { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime OrderDate { get; set; }
        public Status Status { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalGST => Math.Round(TotalCost * (decimal)0.15, 2);
        public decimal SubTotal => Math.Round(TotalCost - TotalGST, 2);
        public List<OrderDetail> OrderDetails { get; set; }
        public ApplicationUser User { get; set; }
    }
}
