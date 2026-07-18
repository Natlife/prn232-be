using System;
using BusinessObjects.Models;
using BusinessObjects.DTOs;
using Repositories;

namespace Services
{
    public class AppointmentConsumedPartService : IAppointmentConsumedPartService
    {
        private readonly IAppointmentConsumedPartRepository _consumedPartRepository;
        private readonly IPartRepository _partRepository;

        public AppointmentConsumedPartService(
            IAppointmentConsumedPartRepository consumedPartRepository,
            IPartRepository partRepository)
        {
            _consumedPartRepository = consumedPartRepository;
            _partRepository = partRepository;
        }

        public void ReportIncurredPart(IncurredPartReportDto dto)
        {
            var part = _partRepository.GetPartById(dto.PartId);
            if (part == null) throw new Exception("Khong tim thay phu tung.");

            var consumedPart = new AppointmentConsumedPart
            {
                AppointmentId = dto.AppointmentId,
                AppointmentDetailId = dto.AppointmentDetailId,
                PartId = dto.PartId,
                Quantity = dto.Quantity,
                UnitPrice = part.Price, // Khóa giá lúc báo cáo
                IsIncurred = true,
                ApprovedByCustomer = false, // Chờ khách duyệt
                Notes = dto.Notes,
                CreatedAt = DateTime.Now
            };

            _consumedPartRepository.AddConsumedPart(consumedPart);
        }

        public void AddPart(IncurredPartReportDto dto)
        {
            var part = _partRepository.GetPartById(dto.PartId);
            if (part == null) throw new Exception("Không tìm thấy phụ tùng.");
            if (part.Quantity < dto.Quantity) throw new Exception("Không đủ số lượng tồn kho.");

            // Trừ kho ngay lập tức
            part.Quantity -= dto.Quantity;
            _partRepository.UpdatePart(part);

            var consumedPart = new AppointmentConsumedPart
            {
                AppointmentId = dto.AppointmentId,
                AppointmentDetailId = dto.AppointmentDetailId,
                PartId = dto.PartId,
                Quantity = dto.Quantity,
                UnitPrice = part.Price, // Khóa giá lúc báo cáo
                IsIncurred = false,
                ApprovedByCustomer = true, // Khách đã duyệt/Admin thêm vào
                Notes = dto.Notes,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _consumedPartRepository.AddConsumedPart(consumedPart);
        }

        public void ApproveIncurredPart(IncurredPartApprovalDto dto)
        {
            var consumedPart = _consumedPartRepository.GetById(dto.ConsumedPartId);
            if (consumedPart == null) throw new Exception("Khong tim thay ban ghi phu tung phat sinh.");
            if (!consumedPart.IsIncurred) throw new Exception("Day la phu tung dinh muc, khong the duyet/tu choi.");
            if (consumedPart.ApprovedByCustomer) throw new Exception("Phu tung nay da duoc duyet truoc do.");

            if (dto.IsApproved)
            {
                // Khách duyệt -> cập nhật trạng thái và trừ kho
                var part = _partRepository.GetPartById(consumedPart.PartId);
                if (part == null) throw new Exception("Khong tim thay phu tung trong kho.");
                if (part.Quantity < consumedPart.Quantity) throw new Exception("Khong du ton kho cho phu tung nay.");

                part.Quantity -= consumedPart.Quantity;
                _partRepository.UpdatePart(part);

                consumedPart.ApprovedByCustomer = true;
                consumedPart.UpdatedAt = DateTime.Now;
                _consumedPartRepository.UpdateConsumedPart(consumedPart);
            }
            else
            {
                // Khách từ chối -> xóa bản ghi phát sinh này
                _consumedPartRepository.DeleteConsumedPart(dto.ConsumedPartId);
            }
        }

        public void RemoveIncurredPart(int consumedPartId)
        {
            var consumedPart = _consumedPartRepository.GetById(consumedPartId);
            if (consumedPart == null) throw new Exception("Không tìm thấy phụ tùng phát sinh.");

            // Nếu đã được duyệt (đã trừ kho), cần hoàn lại số lượng vào kho
            if (consumedPart.ApprovedByCustomer)
            {
                var part = _partRepository.GetPartById(consumedPart.PartId);
                if (part != null)
                {
                    part.Quantity += consumedPart.Quantity;
                    _partRepository.UpdatePart(part);
                }
            }

            _consumedPartRepository.DeleteConsumedPart(consumedPartId);
        }
    }
}
