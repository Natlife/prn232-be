using System;

namespace BusinessObjects.Models;

public partial class ComboOrderItem
{
    public int ItemId { get; set; }

    public int ComboOrderId { get; set; }

    /// <summary>Car | Part | Service</summary>
    public string ItemType { get; set; } = null!;

    /// <summary>CarId | PartId | PackageId depending on ItemType</summary>
    public int ReferenceId { get; set; }

    public string ItemName { get; set; } = null!;

    public int Quantity { get; set; } = 1;

    public decimal UnitPrice { get; set; }

    public decimal SubTotal { get; set; }

    public virtual ComboOrder ComboOrder { get; set; } = null!;
}
