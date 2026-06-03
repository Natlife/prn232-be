using System.Collections.Generic;
using BusinessObjects.Models;
using Repositories;

namespace Services
{
    public class PartService : IPartService
    {
        private readonly IPartRepository _repository;

        public PartService(IPartRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Part> GetAllParts() => _repository.GetAllParts();

        public Part? GetPartById(int partId) => _repository.GetPartById(partId);

        public void AddPart(Part part)
        {
            if (string.IsNullOrWhiteSpace(part.PartName))
            {
                throw new InvalidOperationException("Tên phụ tùng không được để trống.");
            }
            if (string.IsNullOrWhiteSpace(part.PartCode))
            {
                throw new InvalidOperationException("Mã phụ tùng không được để trống.");
            }

            var name = part.PartName.Trim();
            var code = part.PartCode.Trim();
            var parts = _repository.GetAllParts();

            if (parts.Any(p => p.PartName.Trim().Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Tên phụ tùng đã tồn tại trong hệ thống. Vui lòng nhập tên khác.");
            }
            if (parts.Any(p => p.PartCode.Trim().Equals(code, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Mã phụ tùng đã tồn tại trong hệ thống. Vui lòng nhập mã khác.");
            }

            part.PartName = name;
            part.PartCode = code;
            _repository.AddPart(part);
        }

        public void UpdatePart(Part part)
        {
            if (string.IsNullOrWhiteSpace(part.PartName))
            {
                throw new InvalidOperationException("Tên phụ tùng không được để trống.");
            }
            if (string.IsNullOrWhiteSpace(part.PartCode))
            {
                throw new InvalidOperationException("Mã phụ tùng không được để trống.");
            }

            var name = part.PartName.Trim();
            var code = part.PartCode.Trim();
            var parts = _repository.GetAllParts();

            if (parts.Any(p => p.PartId != part.PartId && p.PartName.Trim().Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Tên phụ tùng đã tồn tại trong hệ thống. Vui lòng nhập tên khác.");
            }
            if (parts.Any(p => p.PartId != part.PartId && p.PartCode.Trim().Equals(code, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Mã phụ tùng đã tồn tại trong hệ thống. Vui lòng nhập mã khác.");
            }

            part.PartName = name;
            part.PartCode = code;
            _repository.UpdatePart(part);
        }

        public void DeletePart(int partId)
        {
            var part = _repository.GetPartById(partId);
            if (part == null)
            {
                throw new InvalidOperationException("Không tìm thấy phụ tùng cần xóa.");
            }
            if (part.Quantity > 0)
            {
                throw new InvalidOperationException("Không thể xóa phụ tùng đang có số lượng lớn hơn 0.");
            }
            try
            {
                _repository.DeletePart(partId);
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Không thể xóa phụ tùng này vì đã tồn tại trong lịch sử đơn hàng.");
            }
        }
    }
}
