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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
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
