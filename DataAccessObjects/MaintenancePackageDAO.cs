using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects
{
    public class MaintenancePackageDAO
    {
        private static MaintenancePackageDAO instance = null;
        private static readonly object instanceLock = new object();

        private MaintenancePackageDAO() { }

        public static MaintenancePackageDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new MaintenancePackageDAO();
                    }
                    return instance;
                }
            }
        }

        public IEnumerable<MaintenancePackage> GetAllPackages()
        {
            using var context = new CarShowroomContext();
            return context.MaintenancePackages.ToList();
        }

        public MaintenancePackage GetPackageById(int packageId)
        {
            using var context = new CarShowroomContext();
            return context.MaintenancePackages.SingleOrDefault(p => p.PackageId == packageId);
        }

        public void AddPackage(MaintenancePackage package)
        {
            using var context = new CarShowroomContext();
            context.MaintenancePackages.Add(package);
            context.SaveChanges();
        }

        public void UpdatePackage(MaintenancePackage package)
        {
            using var context = new CarShowroomContext();
            context.Entry(package).State = EntityState.Modified;
            context.SaveChanges();
        }

        public void DeletePackage(int packageId)
        {
            using var context = new CarShowroomContext();
            var package = context.MaintenancePackages.SingleOrDefault(p => p.PackageId == packageId);
            if (package != null)
            {
                context.MaintenancePackages.Remove(package);
                context.SaveChanges();
            }
        }
    }
}
