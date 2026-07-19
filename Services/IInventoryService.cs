using BusinessObjects.DTOs;

namespace Services
{
    public interface IInventoryService
    {
        InventoryReceiptResponseDto CreateInventoryReceipt(InventoryReceiptCreateDto dto, int staffId);
    }
}
