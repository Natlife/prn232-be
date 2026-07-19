using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects.DTOs;
using BusinessObjects.Models;
using Repositories;

namespace Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IPartRepository _partRepository;
        private readonly IPartCategoryRepository _categoryRepository;

        public InventoryService(
            IInventoryRepository inventoryRepository,
            IPartRepository partRepository,
            IPartCategoryRepository categoryRepository)
        {
            _inventoryRepository = inventoryRepository;
            _partRepository = partRepository;
            _categoryRepository = categoryRepository;
        }

        public InventoryReceiptResponseDto CreateInventoryReceipt(InventoryReceiptCreateDto dto, int staffId)
        {
            if (dto.SupplierId <= 0)
            {
                throw new InvalidOperationException("Nhà cung cấp không hợp lệ.");
            }
            if (dto.Items == null || dto.Items.Count == 0)
            {
                throw new InvalidOperationException("Danh sách phụ tùng nhập kho không được để trống.");
            }

            var receipt = new InventoryReceipt
            {
                SupplierId = dto.SupplierId,
                StaffId = staffId,
                Notes = dto.Notes,
                CreatedUser = staffId
            };

            var details = new List<InventoryReceiptDetail>();
            var partExpirations = new Dictionary<int, DateTime?>();

            var allCategories = _categoryRepository.GetAllCategories().ToList();

            foreach (var item in dto.Items)
            {
                if (item.Quantity <= 0)
                {
                    throw new InvalidOperationException("Số lượng nhập kho phải lớn hơn 0.");
                }
                if (item.ImportPrice < 0)
                {
                    throw new InvalidOperationException("Giá nhập kho không được âm.");
                }

                Part? part = null;
                if (item.NewPart != null)
                {
                    var code = item.NewPart.PartCode.Trim();
                    var existingPart = _partRepository.GetAllParts().FirstOrDefault(p => p.PartCode.Trim().Equals(code, StringComparison.OrdinalIgnoreCase));
                    if (existingPart != null)
                    {
                        part = existingPart;
                        item.PartId = existingPart.PartId;
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(item.NewPart.PartName))
                        {
                            throw new InvalidOperationException("Tên phụ tùng mới không được để trống.");
                        }
                        var newPart = new Part
                        {
                            CategoryId = dto.CategoryId,
                            PartName = item.NewPart.PartName.Trim(),
                            PartCode = code,
                            Brand = item.NewPart.Brand?.Trim(),
                            Price = item.NewPart.Price,
                            Quantity = 0,
                            MinStockLevel = item.NewPart.MinStockLevel,
                            MaxStockLevel = item.NewPart.MaxStockLevel,
                            UnitOfMeasure = string.IsNullOrWhiteSpace(item.NewPart.UnitOfMeasure) ? "Cái" : item.NewPart.UnitOfMeasure.Trim(),
                            WarehouseLocation = item.NewPart.WarehouseLocation?.Trim(),
                            WarrantyMonths = item.NewPart.WarrantyMonths,
                            Description = item.NewPart.Description?.Trim(),
                            ImageUrl = item.NewPart.ImageUrl?.Trim(),
                            Status = "Available",
                            CreatedAt = DateTime.Now,
                            CreatedUser = staffId
                        };
                        _partRepository.AddPart(newPart);
                        part = newPart;
                        item.PartId = newPart.PartId;
                    }
                }
                else
                {
                    part = _partRepository.GetPartById(item.PartId);
                    if (part == null)
                    {
                        throw new InvalidOperationException($"Phụ tùng với ID '{item.PartId}' không tồn tại.");
                    }
                }

                // Check shelf life validation (ExpiredAt is mandatory for oils, chemicals, batteries)
                var category = allCategories.FirstOrDefault(c => c.CategoryId == part.CategoryId);
                string catName = category?.CategoryName ?? "";
                bool isPerishable = catName.Contains("dầu", StringComparison.OrdinalIgnoreCase) ||
                                   catName.Contains("nhớt", StringComparison.OrdinalIgnoreCase) ||
                                   catName.Contains("hóa chất", StringComparison.OrdinalIgnoreCase) ||
                                   catName.Contains("ắc quy", StringComparison.OrdinalIgnoreCase) ||
                                   part.CategoryId == 2 || part.CategoryId == 3;

                if (isPerishable)
                {
                    if (!item.ExpiredAt.HasValue)
                    {
                        throw new InvalidOperationException($"Bắt buộc phải nhập Hạn sử dụng (ExpiredAt) cho phụ tùng thuộc nhóm dầu máy, hóa chất, hoặc ắc quy: '{part.PartName}'.");
                    }
                    if (item.ExpiredAt.Value <= DateTime.Now)
                    {
                        throw new InvalidOperationException($"Hạn sử dụng cho phụ tùng '{part.PartName}' phải ở tương lai.");
                    }
                }

                var detail = new InventoryReceiptDetail
                {
                    PartId = item.PartId,
                    Quantity = item.Quantity,
                    ImportPrice = item.ImportPrice,
                    CreatedUser = staffId
                };
                details.Add(detail);

                partExpirations[item.PartId] = item.ExpiredAt;
            }

            var createdReceipt = _inventoryRepository.CreateReceipt(receipt, details, partExpirations);

            // Construct response message
            var firstItem = dto.Items.First();
            var firstPart = _partRepository.GetPartById(firstItem.PartId);
            string message = $"Đã nhập kho {firstItem.Quantity} {firstPart?.UnitOfMeasure ?? "Cái"} {firstPart?.PartName} và tự động cộng dồn tồn kho.";
            if (dto.Items.Count > 1)
            {
                message += $" (Và {dto.Items.Count - 1} mặt hàng khác)";
            }

            return new InventoryReceiptResponseDto
            {
                ReceiptId = createdReceipt.ReceiptId,
                TotalAmount = createdReceipt.TotalAmount,
                Status = "Success",
                Message = message
            };
        }
    }
}
