using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MaoriSouvenirShopping.Models
{
    public enum Status
    {
        Ordered, Billed, Paid, Cancelled
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
        public List<OrderDetail> OrderDetails { get; set; }
        public ApplicationUser User { get; set; }
        //public int CustomerID { get; set; }
        //public Customer Customer { get; set; }
        //public ICollection<OrderItem> OrderItems { get; set; }
    }
}
