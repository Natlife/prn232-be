using System.Collections.Generic;
using BusinessObjects.Models;

namespace Repositories
{
    public interface IMaintenancePackageRepository
    {
        IEnumerable<MaintenancePackage> GetAllPackages();
        MaintenancePackage GetPackageById(int packageId);
        void AddPackage(MaintenancePackage package);
        void UpdatePackage(MaintenancePackage package);
        void DeletePackage(int packageId);
    }
}
