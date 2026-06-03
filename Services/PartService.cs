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

        public void AddPart(Part part) => _repository.AddPart(part);

        public void UpdatePart(Part part) => _repository.UpdatePart(part);

        public void DeletePart(int partId) => _repository.DeletePart(partId);
    }
}
