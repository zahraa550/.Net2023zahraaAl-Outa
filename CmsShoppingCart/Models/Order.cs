using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace CmsShoppingCart.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string phoneNumber { get; set; }

        [Required]
        public string Adress { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a valid positive price.")]
        public decimal TotalAmount { get; set; }

        public string userId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required] public string PaymentMethod { get; set; }

         public List<CartItem> CartItems { get; set; }

         public string Description { get; set; }
    }
}
