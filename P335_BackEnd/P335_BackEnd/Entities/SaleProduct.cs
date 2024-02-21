using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P335_BackEnd.Entities
{
    public class SaleProduct
    {
        public int Id {  get; set; }

        [Column(TypeName = "money")]
        public decimal DiscountedPrice { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

    }
}
