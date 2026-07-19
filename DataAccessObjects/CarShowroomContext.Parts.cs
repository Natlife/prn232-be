using Microsoft.EntityFrameworkCore;
using BusinessObjects.Models;

namespace DataAccessObjects
{
    public partial class CarShowroomContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            // Configure Part extensions
            modelBuilder.Entity<Part>(entity =>
            {
                entity.Property(e => e.MinStockLevel).HasDefaultValue(5);
                entity.Property(e => e.MaxStockLevel).HasDefaultValue(100);
                entity.Property(e => e.UnitOfMeasure).HasMaxLength(20).HasDefaultValue("Cái");
                entity.Property(e => e.WarehouseLocation).HasMaxLength(100);
                entity.Property(e => e.WarrantyMonths).HasDefaultValue(0);
                entity.Property(e => e.ExpiredAt).HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedUserNavigation)
                    .WithMany(p => p.CreatedParts)
                    .HasForeignKey(d => d.CreatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Parts_CreatedUser");

                entity.HasOne(d => d.UpdatedUserNavigation)
                    .WithMany(p => p.UpdatedParts)
                    .HasForeignKey(d => d.UpdatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Parts_UpdatedUser");
            });

            // Configure PartCategory extensions
            modelBuilder.Entity<PartCategory>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("(getdate())");
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedUserNavigation)
                    .WithMany(p => p.CreatedPartCategories)
                    .HasForeignKey(d => d.CreatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartCategories_CreatedUser");

                entity.HasOne(d => d.UpdatedUserNavigation)
                    .WithMany(p => p.UpdatedPartCategories)
                    .HasForeignKey(d => d.UpdatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartCategories_UpdatedUser");
            });

            // Configure Supplier
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(e => e.SupplierId);
                entity.ToTable("Suppliers");

                entity.Property(e => e.SupplierName).HasMaxLength(150).IsRequired();
                entity.Property(e => e.ContactName).HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Address).HasMaxLength(255);
                entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("Active");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("(getdate())");
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedUserNavigation)
                    .WithMany(p => p.CreatedSuppliers)
                    .HasForeignKey(d => d.CreatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Suppliers_CreatedUser");

                entity.HasOne(d => d.UpdatedUserNavigation)
                    .WithMany(p => p.UpdatedSuppliers)
                    .HasForeignKey(d => d.UpdatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Suppliers_UpdatedUser");
            });

            // Configure PartCompatibility
            modelBuilder.Entity<PartCompatibility>(entity =>
            {
                entity.HasKey(e => e.CompatibilityId);
                entity.ToTable("PartCompatibilities");

                entity.Property(e => e.ModelName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("(getdate())");
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Part)
                    .WithMany(p => p.PartCompatibilities)
                    .HasForeignKey(d => d.PartId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_PartCompatibilities_Parts");

                entity.HasOne(d => d.Brand)
                    .WithMany()
                    .HasForeignKey(d => d.BrandId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartCompatibilities_CarBrands");

                entity.HasOne(d => d.CreatedUserNavigation)
                    .WithMany(p => p.CreatedPartCompatibilities)
                    .HasForeignKey(d => d.CreatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartCompatibilities_CreatedUser");

                entity.HasOne(d => d.UpdatedUserNavigation)
                    .WithMany(p => p.UpdatedPartCompatibilities)
                    .HasForeignKey(d => d.UpdatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartCompatibilities_UpdatedUser");
            });

            // Configure InventoryReceipt
            modelBuilder.Entity<InventoryReceipt>(entity =>
            {
                entity.HasKey(e => e.ReceiptId);
                entity.ToTable("InventoryReceipts");

                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)").HasDefaultValue(0);
                entity.Property(e => e.ReceiptDate).HasColumnType("datetime").HasDefaultValueSql("(getdate())");
                entity.Property(e => e.Notes).HasMaxLength(1000);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("(getdate())");
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.InventoryReceipts)
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_InventoryReceipts_Suppliers");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.StaffReceipts)
                    .HasForeignKey(d => d.StaffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_InventoryReceipts_AppUsers");

                entity.HasOne(d => d.CreatedUserNavigation)
                    .WithMany(p => p.CreatedReceipts)
                    .HasForeignKey(d => d.CreatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_InventoryReceipts_CreatedUser");

                entity.HasOne(d => d.UpdatedUserNavigation)
                    .WithMany(p => p.UpdatedReceipts)
                    .HasForeignKey(d => d.UpdatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_InventoryReceipts_UpdatedUser");
            });

            // Configure InventoryReceiptDetail
            modelBuilder.Entity<InventoryReceiptDetail>(entity =>
            {
                entity.HasKey(e => e.ReceiptDetailId);
                entity.ToTable("InventoryReceiptDetails");

                entity.Property(e => e.ImportPrice).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.SubTotal)
                    .HasColumnType("decimal(18, 2)")
                    .HasComputedColumnSql("([Quantity]*[ImportPrice])", stored: false); // Computed in database

                entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("(getdate())");
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Receipt)
                    .WithMany(p => p.InventoryReceiptDetails)
                    .HasForeignKey(d => d.ReceiptId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_InventoryReceiptDetails_Receipts");

                entity.HasOne(d => d.Part)
                    .WithMany(p => p.InventoryReceiptDetails)
                    .HasForeignKey(d => d.PartId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_InventoryReceiptDetails_Parts");

                entity.HasOne(d => d.CreatedUserNavigation)
                    .WithMany(p => p.CreatedReceiptDetails)
                    .HasForeignKey(d => d.CreatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_InventoryReceiptDetails_CreatedUser");

                entity.HasOne(d => d.UpdatedUserNavigation)
                    .WithMany(p => p.UpdatedReceiptDetails)
                    .HasForeignKey(d => d.UpdatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_InventoryReceiptDetails_UpdatedUser");
            });

            // Configure InventoryTransaction
            modelBuilder.Entity<InventoryTransaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId);
                entity.ToTable("InventoryTransactions");

                entity.Property(e => e.TransactionType).HasMaxLength(20).IsRequired();
                entity.Property(e => e.ReferenceType).HasMaxLength(50);
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.TransactionDate).HasColumnType("datetime").HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("(getdate())");
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Part)
                    .WithMany(p => p.InventoryTransactions)
                    .HasForeignKey(d => d.PartId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_InventoryTransactions_Parts");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.StaffTransactions)
                    .HasForeignKey(d => d.StaffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_InventoryTransactions_AppUsers");

                entity.HasOne(d => d.CreatedUserNavigation)
                    .WithMany(p => p.CreatedTransactions)
                    .HasForeignKey(d => d.CreatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_InventoryTransactions_CreatedUser");

                entity.HasOne(d => d.UpdatedUserNavigation)
                    .WithMany(p => p.UpdatedTransactions)
                    .HasForeignKey(d => d.UpdatedUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_InventoryTransactions_UpdatedUser");
            });

            // Configure PartOrder master invoice relation mapping
            modelBuilder.Entity<PartOrder>(entity =>
            {
                // Verify/update the custom database mapping
                entity.Property(e => e.MasterInvoiceId).HasColumnName("MasterInvoiceId");
            });

            // Configure CustomerCar
            modelBuilder.Entity<CustomerCar>(entity =>
            {
                entity.HasKey(e => e.CustomerCarId);
                entity.ToTable("CustomerCars");

                entity.Property(e => e.Model).HasMaxLength(100).IsRequired();
                entity.Property(e => e.VIN).HasMaxLength(50);
                entity.HasIndex(e => e.VIN).IsUnique();
                entity.Property(e => e.LicensePlate).HasMaxLength(30).IsRequired();
                entity.HasIndex(e => e.LicensePlate).IsUnique();
                entity.Property(e => e.Color).HasMaxLength(50);
                entity.Property(e => e.ExpiredAt).HasColumnType("datetime");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("(getdate())");
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Customer)
                    .WithMany()
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerCars_AppUsers");

                entity.HasOne(d => d.Brand)
                    .WithMany()
                    .HasForeignKey(d => d.BrandId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerCars_CarBrands");
            });
        }
    }
}
