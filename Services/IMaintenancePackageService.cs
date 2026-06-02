using System.Collections.Generic;
using BusinessObjects.Models;

namespace Services
{
    public interface IMaintenancePackageService
    {
        IEnumerable<MaintenancePackage> GetAllPackages();
        IEnumerable<MaintenancePackage> GetAvailablePackages();
        MaintenancePackage GetPackageById(int packageId);
        void AddPackage(MaintenancePackage package);
        void UpdatePackage(MaintenancePackage package);
        void DeletePackage(int packageId);
    }
}
