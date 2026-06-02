using System.Collections.Generic;
using System.Linq;
using BusinessObjects.Models;
using Repositories;

namespace Services
{
    public class MaintenancePackageService : IMaintenancePackageService
    {
        private readonly IMaintenancePackageRepository _repository;

        public MaintenancePackageService(IMaintenancePackageRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<MaintenancePackage> GetAllPackages() => _repository.GetAllPackages();

        public IEnumerable<MaintenancePackage> GetAvailablePackages() 
            => _repository.GetAllPackages().Where(p => p.Status == "Available");

        public MaintenancePackage GetPackageById(int packageId) => _repository.GetPackageById(packageId);

        public void AddPackage(MaintenancePackage package) => _repository.AddPackage(package);

        public void UpdatePackage(MaintenancePackage package) => _repository.UpdatePackage(package);

        public void DeletePackage(int packageId) => _repository.DeletePackage(packageId);
    }
}
