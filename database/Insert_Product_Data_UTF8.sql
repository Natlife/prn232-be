-- =========================================================================
-- SCRIPT THÊM DỮ LIỆU SẢN PHẨM MỚI CHUẨN UNICODE (KHÔNG LỖI FONT CHỮ)
-- Hỗ trợ 3 Module: 
--   1. Ô tô (Cars)
--   2. Phụ tùng (Parts)
--   3. Dịch vụ & Gói bảo dưỡng (Services & MaintenancePackages)
-- =========================================================================

USE CarShowroomDB;
GO

-- ─────────────────────────────────────────────────────────────────────────
-- GIẢI THÍCH NGUYÊN NHÂN LỖI FONT CHỮ TRONG SQL SERVER & CÁCH SỬA:
-- 1. Trong SQL Server, các cột kiểu NVARCHAR cần phải có tiền tố N'...' khi INSERT.
--    Ví dụ: N'Xe sang trọng' thay vì 'Xe sang trọng'. Nếu thiếu chữ N, SQL Server sẽ
--    chuyển chuỗi về dạng VARCHAR (ASCII/Windows-1252) khiến các dấu tiếng Việt bị thành dấu '?' hoặc 'Ã¡'.
-- 2. File script cần được lưu ở định dạng UTF-8 với BOM (Byte Order Mark) để SSMS / sqlcmd nhận diện đúng font.
-- ─────────────────────────────────────────────────────────────────────────

-- 1. ĐẢM BẢO CÓ ĐỦ CAR BRANDS (HÃNG XE)
IF NOT EXISTS (SELECT 1 FROM CarBrands WHERE BrandName = 'Porsche')
    INSERT INTO CarBrands (BrandName, Country, Description, CreatedUser) VALUES (N'Porsche', N'Đức', N'Hãng xe thể thao cao cấp Đức', 1);

IF NOT EXISTS (SELECT 1 FROM CarBrands WHERE BrandName = 'Hyundai')
    INSERT INTO CarBrands (BrandName, Country, Description, CreatedUser) VALUES (N'Hyundai', N'Hàn Quốc', N'Tập đoàn ô tô Hyundai Hàn Quốc', 1);

IF NOT EXISTS (SELECT 1 FROM CarBrands WHERE BrandName = 'Mercedes-Benz')
    INSERT INTO CarBrands (BrandName, Country, Description, CreatedUser) VALUES (N'Mercedes-Benz', N'Đức', N'Hãng xe hạng sang Mercedes-Benz Đức', 1);


-- 2. ĐẢM BẢO CÓ ĐỦ PART CATEGORIES (DANH MỤC PHỤ TÙNG)
IF NOT EXISTS (SELECT 1 FROM PartCategories WHERE CategoryName LIKE N'%Phanh%')
    INSERT INTO PartCategories (CategoryName, Description, CreatedUser) VALUES (N'Hệ thống Phanh & Gầm', N'Má phanh, đĩa phanh, giảm xóc và thước lái.', 1);

IF NOT EXISTS (SELECT 1 FROM PartCategories WHERE CategoryName LIKE N'%Chiếu sáng%')
    INSERT INTO PartCategories (CategoryName, Description, CreatedUser) VALUES (N'Hệ thống Chiếu sáng & Đèn', N'Đèn pha LED, đèn sương mù và bóng đèn.', 1);


-- =========================================================================
-- MODULE 1: THÊM DỮ LIỆU SẢN PHẨM Ô TÔ (CARS)
-- =========================================================================
PRINT N'Đang thêm dữ liệu Ô tô...';

IF NOT EXISTS (SELECT 1 FROM Cars WHERE CarName LIKE N'%VinFast VF9 Eco%')
INSERT INTO Cars (BrandId, CarName, Model, [Year], Color, Mileage, FuelType, Transmission, Price, Description, ImageUrl, Status, CreatedUser)
VALUES 
((SELECT TOP 1 BrandId FROM CarBrands WHERE BrandName = 'VinFast'), 
 N'VinFast VF9 Eco', N'VF9', 2024, N'Trắng', 0, N'Electric', N'Automatic', 1491000000.00, 
 N'Mẫu SUV điện 7 chỗ cỡ lớn hạng E sang trọng bậc nhất của VinFast, trang bị 2 motor điện 402 mã lực, quãng đường di chuyển tới 438km/lần sạc.', 
 N'https://images.unsplash.com/photo-1617788138017-80ad40651399?auto=format&fit=crop&w=800&q=80', 'Available', 1);

IF NOT EXISTS (SELECT 1 FROM Cars WHERE CarName LIKE N'%BMW 730Li%')
INSERT INTO Cars (BrandId, CarName, Model, [Year], Color, Mileage, FuelType, Transmission, Price, Description, ImageUrl, Status, CreatedUser)
VALUES 
((SELECT TOP 1 BrandId FROM CarBrands WHERE BrandName = 'BMW'), 
 N'BMW 730Li M Sport', N'730Li', 2023, N'Đen', 12000, N'Gasoline', N'Automatic', 4499000000.00, 
 N'Sedan hạng sang cỡ lớn đỉnh cao từ Đức, trang bị gói nội thất da Nappa cao cấp, âm thanh Harman Kardon và đèn laser hiện đại.', 
 N'https://images.unsplash.com/photo-1555215695-3004980ad54e?auto=format&fit=crop&w=800&q=80', 'Available', 1);

IF NOT EXISTS (SELECT 1 FROM Cars WHERE CarName LIKE N'%Porsche Cayenne%')
INSERT INTO Cars (BrandId, CarName, Model, [Year], Color, Mileage, FuelType, Transmission, Price, Description, ImageUrl, Status, CreatedUser)
VALUES 
((SELECT TOP 1 BrandId FROM CarBrands WHERE BrandName = 'Porsche'), 
 N'Porsche Cayenne Coupe 3.0 V6', N'Cayenne', 2023, N'Xám', 9500, N'Gasoline', N'Automatic', 5560000000.00, 
 N'SUV thể thao hạng sang biểu tượng của Porsche, động cơ V6 Turbo 340 mã lực, khả năng tăng tốc 0-100km/h chỉ trong 5.9 giây.', 
 N'https://images.unsplash.com/photo-1503376780353-7e6692767b70?auto=format&fit=crop&w=800&q=80', 'Available', 1);

IF NOT EXISTS (SELECT 1 FROM Cars WHERE CarName LIKE N'%Hyundai SantaFe%')
INSERT INTO Cars (BrandId, CarName, Model, [Year], Color, Mileage, FuelType, Transmission, Price, Description, ImageUrl, Status, CreatedUser)
VALUES 
((SELECT TOP 1 BrandId FROM CarBrands WHERE BrandName = 'Hyundai'), 
 N'Hyundai SantaFe 2.2 Dầu Cao Cấp', N'SantaFe', 2022, N'Đỏ', 28000, N'Diesel', N'Automatic', 1150000000.00, 
 N'SUV 7 chỗ gia đình máy dầu tiết kiệm nhiên liệu, trang bị dẫn động 4 bánh HTRAC, cửa sổ trời toàn cảnh Panorama và tính năng cảnh báo điểm mù.', 
 N'https://images.unsplash.com/photo-1549399542-7e3f8b79c341?auto=format&fit=crop&w=800&q=80', 'Available', 1);

IF NOT EXISTS (SELECT 1 FROM Cars WHERE CarName LIKE N'%Mercedes-Maybach S450%')
INSERT INTO Cars (BrandId, CarName, Model, [Year], Color, Mileage, FuelType, Transmission, Price, Description, ImageUrl, Status, CreatedUser)
VALUES 
((SELECT TOP 1 BrandId FROM CarBrands WHERE BrandName = 'Mercedes-Benz'), 
 N'Mercedes-Maybach S450 4MATIC', N'S450', 2023, N'Đen mờ', 4500, N'Gasoline', N'Automatic', 8199000000.00, 
 N'Tuyệt phẩm xe siêu sang cho chủ nhân thượng lưu, tích hợp ghế thương gia massage đá nóng, tủ lạnh mini và hệ thống treo khí nén AIRMATIC.', 
 N'https://images.unsplash.com/photo-1618843479313-40f8afb4b4d8?auto=format&fit=crop&w=800&q=80', 'Available', 1);


-- =========================================================================
-- MODULE 2: THÊM DỮ LIỆU PHỤ TÙNG Ô TÔ (PARTS)
-- =========================================================================
PRINT N'Đang thêm dữ liệu Phụ tùng...';

IF NOT EXISTS (SELECT 1 FROM Parts WHERE PartCode = 'PT-BREM-CERAMIC')
INSERT INTO Parts (CategoryId, PartName, PartCode, Brand, Price, Quantity, MinStockLevel, MaxStockLevel, UnitOfMeasure, WarehouseLocation, WarrantyMonths, Description, ImageUrl, Status, ExpiredAt, CreatedUser)
VALUES 
((SELECT TOP 1 CategoryId FROM PartCategories WHERE CategoryName LIKE N'%Phanh%'), 
 N'Bộ Má Phanh Ceramic Brembo Xtra', 'PT-BREM-CERAMIC', 'Brembo', 4850000.00, 30, 5, 80, N'Bộ', N'Khu C - Kệ 2 - Ngăn 1', 12, 
 N'Má phanh gốm cao cấp chịu nhiệt tốt, giảm bụi phanh và không phát tiếng ồn khi phanh gấp. Tương thích với các dòng xe sang.', 
 N'https://images.unsplash.com/photo-1600706432523-988185b0e017?auto=format&fit=crop&w=800&q=80', 'Available', '2032-12-31', 1);

IF NOT EXISTS (SELECT 1 FROM Parts WHERE PartCode = 'PT-KN-AIRFILTER')
INSERT INTO Parts (CategoryId, PartName, PartCode, Brand, Price, Quantity, MinStockLevel, MaxStockLevel, UnitOfMeasure, WarehouseLocation, WarrantyMonths, Description, ImageUrl, Status, ExpiredAt, CreatedUser)
VALUES 
((SELECT TOP 1 CategoryId FROM PartCategories WHERE CategoryName LIKE N'%Động cơ%'), 
 N'Lọc Gió Động Cơ Thể Thao K&N High-Flow', 'PT-KN-AIRFILTER', 'K&N', 2150000.00, 45, 10, 100, N'Cái', N'Khu B - Kệ 4 - Ngăn 3', 24, 
 N'Lọc gió rửa tái sử dụng trọn đời, tăng luồng không khí nạp vào động cơ giúp tối ưu công suất và gia tốc cho xe.', 
 N'https://images.unsplash.com/photo-1580273916550-e323be2ae537?auto=format&fit=crop&w=800&q=80', 'Available', '2035-01-01', 1);

IF NOT EXISTS (SELECT 1 FROM Parts WHERE PartCode = 'PT-PHI-LED9000')
INSERT INTO Parts (CategoryId, PartName, PartCode, Brand, Price, Quantity, MinStockLevel, MaxStockLevel, UnitOfMeasure, WarehouseLocation, WarrantyMonths, Description, ImageUrl, Status, ExpiredAt, CreatedUser)
VALUES 
((SELECT TOP 1 CategoryId FROM PartCategories WHERE CategoryName LIKE N'%Chiếu sáng%'), 
 N'Bộ Đèn LED Pha Philips Ultinon Pro9000 H7', 'PT-PHI-LED9000', 'Philips', 3600000.00, 25, 5, 50, N'Cặp', N'Khu A - Kệ 3 - Ngăn 2', 36, 
 N'Đèn pha LED siêu sáng màu trắng ánh sáng ban ngày 5800K, độ sáng tăng 250%, thiết kế tản nhiệt AirBoost chống nóng.', 
 N'https://images.unsplash.com/photo-1511919884226-fd3cad34687c?auto=format&fit=crop&w=800&q=80', 'Available', '2031-06-30', 1);

IF NOT EXISTS (SELECT 1 FROM Parts WHERE PartCode = 'PT-NGK-IRIDIUM')
INSERT INTO Parts (CategoryId, PartName, PartCode, Brand, Price, Quantity, MinStockLevel, MaxStockLevel, UnitOfMeasure, WarehouseLocation, WarrantyMonths, Description, ImageUrl, Status, ExpiredAt, CreatedUser)
VALUES 
((SELECT TOP 1 CategoryId FROM PartCategories WHERE CategoryName LIKE N'%Động cơ%'), 
 N'Bugi Bạch Kim NGK Laser Iridium IX', 'PT-NGK-IRIDIUM', 'NGK', 350000.00, 120, 20, 300, N'Cái', N'Khu B - Kệ 2 - Ngăn 5', 12, 
 N'Bugi chấu bạch kim siêu bền giúp đánh lửa cực mạnh, tiết kiệm nhiên liệu và hạn chế đóng cặn cacbon trong buồng đốt.', 
 N'https://images.unsplash.com/photo-1617814076367-b759c7d7e738?auto=format&fit=crop&w=800&q=80', 'Available', '2030-10-15', 1);


-- =========================================================================
-- MODULE 3: THÊM DỮ LIỆU DỊCH VỤ LẺ & GÓI BẢO DƯỠNG (SERVICES & PACKAGES)
-- =========================================================================
PRINT N'Đang thêm dữ liệu Dịch vụ & Gói bảo dưỡng...';

-- 3A. DỊCH VỤ LẺ (SERVICES)
IF NOT EXISTS (SELECT 1 FROM Services WHERE ServiceName LIKE N'%Ceramic%')
INSERT INTO Services (ServiceName, Description, BasePrice, EstimatedDurationMinutes, Status, CreatedUser)
VALUES 
(N'Phủ Ceramic 9H Bảo Vệ Sơn Xe Cao Cấp', 
 N'Phủ 3 lớp Ceramic độ cứng 9H chống trầy xước nhẹ, chống tia UV gây phai màu sơn và tạo hiệu ứng lá sen trôi nước chống bám bẩn.', 
 5500000.00, 180, 'Available', 1);

IF NOT EXISTS (SELECT 1 FROM Services WHERE ServiceName LIKE N'%Phim Cách Nhiệt%')
INSERT INTO Services (ServiceName, Description, BasePrice, EstimatedDurationMinutes, Status, CreatedUser)
VALUES 
(N'Dán Phim Cách Nhiệt 3M Crystalline Toàn Xe', 
 N'Dán phim cách nhiệt quang học 200 lớp từ Mỹ, cản 99% tia hồng ngoại và cực tím, giảm nhiệt độ cabin ô tô tới 10-15 độ C.', 
 12800000.00, 150, 'Available', 1);

IF NOT EXISTS (SELECT 1 FROM Services WHERE ServiceName LIKE N'%Đánh Bóng%')
INSERT INTO Services (ServiceName, Description, BasePrice, EstimatedDurationMinutes, Status, CreatedUser)
VALUES 
(N'Đánh Bóng & Chăm Sóc Sơn Xe Chuyên Sâu', 
 N'Xử lý quầng xoáy sơn, đánh bóng 3 bước bằng dung dịch Meguiar''s Mỹ khôi phục độ bóng sáng như xe mới xuất xưởng.', 
 1800000.00, 120, 'Available', 1);

-- 3B. GÓI BẢO DƯỠNG COMBO (MAINTENANCE PACKAGES)
IF NOT EXISTS (SELECT 1 FROM MaintenancePackages WHERE PackageName LIKE N'%40.000km%')
INSERT INTO MaintenancePackages (PackageName, Description, PackagePrice, Status, CreatedUser)
VALUES 
(N'Gói Bảo Dưỡng Toàn Diện Chuyên Sâu 40.000km', 
 N'Combo bảo dưỡng đại tu lớn mốc 40.000km gồm: Thay dầu máy, thay lọc dầu, thay lọc gió động cơ & cabin, bảo dưỡng phanh 4 bánh, thay dầu phanh và kiểm tra 50 hạng mục an toàn.', 
 3850000.00, 'Available', 1);

IF NOT EXISTS (SELECT 1 FROM MaintenancePackages WHERE PackageName LIKE N'%VIP%')
INSERT INTO MaintenancePackages (PackageName, Description, PackagePrice, Status, CreatedUser)
VALUES 
(N'Gói Tân Trang Ngoại Thất & Bảo Vệ Sơn VIP', 
 N'Combo chăm sóc xe cao cấp bao gồm: Rửa xe khoang máy bằng hơi nước nóng, đánh bóng xóa xước 3 bước và phủ 2 lớp Ceramic độ bền 2 năm.', 
 6200000.00, 'Available', 1);

-- Liên kết các Dịch vụ lẻ vào Gói Bảo dưỡng vừa tạo
DECLARE @Pkg1Id INT = (SELECT TOP 1 PackageId FROM MaintenancePackages WHERE PackageName LIKE N'%40.000km%');
DECLARE @Pkg2Id INT = (SELECT TOP 1 PackageId FROM MaintenancePackages WHERE PackageName LIKE N'%VIP%');

DECLARE @SvcThayDau INT = (SELECT TOP 1 ServiceId FROM Services WHERE ServiceName LIKE N'%Thay dầu%');
DECLARE @SvcDanhBong INT = (SELECT TOP 1 ServiceId FROM Services WHERE ServiceName LIKE N'%Đánh B%');
DECLARE @SvcCeramic INT = (SELECT TOP 1 ServiceId FROM Services WHERE ServiceName LIKE N'%Ceramic%');

IF @Pkg1Id IS NOT NULL AND @SvcThayDau IS NOT NULL AND NOT EXISTS (SELECT 1 FROM PackageServices WHERE PackageId = @Pkg1Id AND ServiceId = @SvcThayDau)
    INSERT INTO PackageServices (PackageId, ServiceId, CreatedUser) VALUES (@Pkg1Id, @SvcThayDau, 1);

IF @Pkg2Id IS NOT NULL AND @SvcDanhBong IS NOT NULL AND NOT EXISTS (SELECT 1 FROM PackageServices WHERE PackageId = @Pkg2Id AND ServiceId = @SvcDanhBong)
    INSERT INTO PackageServices (PackageId, ServiceId, CreatedUser) VALUES (@Pkg2Id, @SvcDanhBong, 1);

IF @Pkg2Id IS NOT NULL AND @SvcCeramic IS NOT NULL AND NOT EXISTS (SELECT 1 FROM PackageServices WHERE PackageId = @Pkg2Id AND ServiceId = @SvcCeramic)
    INSERT INTO PackageServices (PackageId, ServiceId, CreatedUser) VALUES (@Pkg2Id, @SvcCeramic, 1);


-- =========================================================================
-- CỬA SỔ SỬA LỖI FONT CHỮ TRÊN DỮ LIỆU CŨ (REPAIR MOJIBAKE DATA IF ANY)
-- =========================================================================
PRINT N'Đang rà soát và tự động sửa các chữ tiếng Việt bị lỗi font ở dữ liệu cũ...';

-- Sửa tên xe nếu bị lỗi font cũ
UPDATE Cars SET CarName = N'Toyota Camry 2.5Q', Description = N'Xe sang trọng, gia đình sử dụng kỹ, bảo dưỡng chính hãng.' WHERE CarName LIKE '%Camry%';
UPDATE Cars SET CarName = N'Ford Ranger Wildtrak 2.0L', Description = N'Vua bán tải, phiên bản cao cấp nhất Wildtrak 2 cầu, đầy đủ công nghệ.' WHERE CarName LIKE '%Ranger%';
UPDATE Cars SET CarName = N'VinFast VF8 Plus', Description = N'Xe điện thông minh Việt Nam, bản Plus pin SDI, công nghệ ADAS hiện đại.' WHERE CarName LIKE '%VF8%';

-- Sửa tên danh mục phụ tùng
UPDATE PartCategories SET CategoryName = N'Động cơ & Truyền động', Description = N'Các bộ phận liên quan đến động cơ, hộp số và truyền động.' WHERE CategoryName LIKE '%ng c%';
UPDATE PartCategories SET CategoryName = N'Hệ thống điện & Ắc quy', Description = N'Ắc quy, máy phát điện, đèn và hệ thống điện.' WHERE CategoryName LIKE '%quy%';
UPDATE PartCategories SET CategoryName = N'Dầu nhớt & Hóa chất', Description = N'Dầu máy, nước làm mát, dầu phanh và hóa chất bảo dưỡng.' WHERE CategoryName LIKE '%hóa chất%';
UPDATE PartCategories SET CategoryName = N'Ngoại thất & Phụ kiện', Description = N'Lốp xe, gạt mưa, gương và các phụ kiện trang trí ngoại thất.' WHERE CategoryName LIKE '%kiện%';

PRINT N'=====> HOÀN TẤT THÊM DỮ LIỆU CẢ 3 MODULE VÀ FIX LỖI FONT CHỮ UNICODE! <=====';
GO
