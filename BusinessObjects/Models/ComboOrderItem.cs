using System;

namespace BusinessObjects.Models
{
    public partial class ComboOrderItem
    {
        public int ItemId { get; set; }
        public int ComboOrderId { get; set; }
        public string ItemType { get; set; } = null!;
        public int ReferenceId { get; set; }
        public string ItemName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }

        public virtual ComboOrder ComboOrder { get; set; } = null!;
    }
}
