-- =============================================
-- SEED DATA FOR CAR MODELS IMAGE DETECTION
-- Run this script against CarShowroomDB to add car data
-- that matches predicted classes from dima806/car_models_image_detection
-- =============================================

USE CarShowroomDB;
GO

-- 1. Ensure Brands Exist
IF NOT EXISTS (SELECT 1 FROM CarBrands WHERE BrandName = 'Toyota')
    INSERT INTO CarBrands (BrandName, Country, Description) VALUES ('Toyota', 'Japan', 'Toyota Motor Corporation');

IF NOT EXISTS (SELECT 1 FROM CarBrands WHERE BrandName = 'BMW')
    INSERT INTO CarBrands (BrandName, Country, Description) VALUES ('BMW', 'Germany', 'Bayerische Motoren Werke AG');

IF NOT EXISTS (SELECT 1 FROM CarBrands WHERE BrandName = 'Ford')
    INSERT INTO CarBrands (BrandName, Country, Description) VALUES ('Ford', 'USA', 'Ford Motor Company');

IF NOT EXISTS (SELECT 1 FROM CarBrands WHERE BrandName = 'Tesla')
    INSERT INTO CarBrands (BrandName, Country, Description) VALUES ('Tesla', 'USA', 'Tesla, Inc. Electric Vehicles');

IF NOT EXISTS (SELECT 1 FROM CarBrands WHERE BrandName = 'Honda')
    INSERT INTO CarBrands (BrandName, Country, Description) VALUES ('Honda', 'Japan', 'Honda Motor Co., Ltd.');

IF NOT EXISTS (SELECT 1 FROM CarBrands WHERE BrandName = 'Audi')
    INSERT INTO CarBrands (BrandName, Country, Description) VALUES ('Audi', 'Germany', 'Audi AG - Premium Vehicles');

IF NOT EXISTS (SELECT 1 FROM CarBrands WHERE BrandName = 'Mercedes-Benz')
    INSERT INTO CarBrands (BrandName, Country, Description) VALUES ('Mercedes-Benz', 'Germany', 'Mercedes-Benz Group AG');

IF NOT EXISTS (SELECT 1 FROM CarBrands WHERE BrandName = 'Porsche')
    INSERT INTO CarBrands (BrandName, Country, Description) VALUES ('Porsche', 'Germany', 'Dr. Ing. h.c. F. Porsche AG');

IF NOT EXISTS (SELECT 1 FROM CarBrands WHERE BrandName = 'Mazda')
    INSERT INTO CarBrands (BrandName, Country, Description) VALUES ('Mazda', 'Japan', 'Mazda Motor Corporation');

IF NOT EXISTS (SELECT 1 FROM CarBrands WHERE BrandName = 'Hyundai')
    INSERT INTO CarBrands (BrandName, Country, Description) VALUES ('Hyundai', 'South Korea', 'Hyundai Motor Company');

IF NOT EXISTS (SELECT 1 FROM CarBrands WHERE BrandName = 'Chevrolet')
    INSERT INTO CarBrands (BrandName, Country, Description) VALUES ('Chevrolet', 'USA', 'General Motors - Chevrolet Division');
GO

-- 2. Insert Cars matching HuggingFace Model Classes
DECLARE @ToyotaId INT = (SELECT BrandId FROM CarBrands WHERE BrandName = 'Toyota');
DECLARE @BmwId INT = (SELECT BrandId FROM CarBrands WHERE BrandName = 'BMW');
DECLARE @FordId INT = (SELECT BrandId FROM CarBrands WHERE BrandName = 'Ford');
DECLARE @TeslaId INT = (SELECT BrandId FROM CarBrands WHERE BrandName = 'Tesla');
DECLARE @HondaId INT = (SELECT BrandId FROM CarBrands WHERE BrandName = 'Honda');
DECLARE @AudiId INT = (SELECT BrandId FROM CarBrands WHERE BrandName = 'Audi');
DECLARE @MercId INT = (SELECT BrandId FROM CarBrands WHERE BrandName = 'Mercedes-Benz');
DECLARE @PorscheId INT = (SELECT BrandId FROM CarBrands WHERE BrandName = 'Porsche');
DECLARE @MazdaId INT = (SELECT BrandId FROM CarBrands WHERE BrandName = 'Mazda');
DECLARE @HyundaiId INT = (SELECT BrandId FROM CarBrands WHERE BrandName = 'Hyundai');
DECLARE @ChevyId INT = (SELECT BrandId FROM CarBrands WHERE BrandName = 'Chevrolet');

-- Toyota Camry (Model: "Toyota Camry" in HF)
IF NOT EXISTS (SELECT 1 FROM Cars WHERE CarName = 'Toyota Camry 2.5Q Hybrid')
    INSERT INTO Cars (BrandId, CarName, Model, [Year], Color, Mileage, FuelType, Transmission, Price, Description, ImageUrl, Status, CreatedAt)
    VALUES (@ToyotaId, 'Toyota Camry 2.5Q Hybrid', 'Camry', 2023, 'White', 12000, 'Hybrid', 'Automatic', 1450000000, 
            'Toyota Camry bản Hybrid 2.5Q cực kỳ êm ái, tiết kiệm nhiên liệu, trang bị gói an toàn Toyota Safety Sense cao cấp.', 
            'https://images.unsplash.com/photo-1621007947382-bb3c3994e3fb?auto=format&fit=crop&w=600&q=80', 'Available', GETDATE());

-- Tesla Model 3 (Model: "Tesla Model 3" in HF)
IF NOT EXISTS (SELECT 1 FROM Cars WHERE CarName = 'Tesla Model 3 Long Range')
    INSERT INTO Cars (BrandId, CarName, Model, [Year], Color, Mileage, FuelType, Transmission, Price, Description, ImageUrl, Status, CreatedAt)
    VALUES (@TeslaId, 'Tesla Model 3 Long Range', 'Model 3', 2022, 'Red', 8000, 'Electric', 'Automatic', 1650000000, 
            'Xe điện thông minh Tesla Model 3 bản Long Range nhập khẩu nguyên chiếc. Tự động lái Autopilot, nội thất tối giản hiện đại.', 
            'https://images.unsplash.com/photo-1619767886558-efdc259cde1a?auto=format&fit=crop&w=600&q=80', 'Available', GETDATE());

-- Tesla Model Y (Model: "Tesla Model Y" in HF)
IF NOT EXISTS (SELECT 1 FROM Cars WHERE CarName = 'Tesla Model Y Performance')
    INSERT INTO Cars (BrandId, CarName, Model, [Year], Color, Mileage, FuelType, Transmission, Price, Description, ImageUrl, Status, CreatedAt)
    VALUES (@TeslaId, 'Tesla Model Y Performance', 'Model Y', 2023, 'Black', 4000, 'Electric', 'Automatic', 1950000000, 
            'SUV điện Tesla Model Y bản Performance gia tốc vượt trội từ 0-100 km/h chỉ 3.7 giây. Quãng đường di chuyển ấn tượng.', 
            'https://images.unsplash.com/photo-1620891549027-942fdc95d3f5?auto=format&fit=crop&w=600&q=80', 'Available', GETDATE());

-- Honda Civic (Model: "Honda Civic" in HF)
IF NOT EXISTS (SELECT 1 FROM Cars WHERE CarName = 'Honda Civic 1.5 RS')
    INSERT INTO Cars (BrandId, CarName, Model, [Year], Color, Mileage, FuelType, Transmission, Price, Description, ImageUrl, Status, CreatedAt)
    VALUES (@HondaId, 'Honda Civic 1.5 RS', 'Civic', 2022, 'Red', 16000, 'Gasoline', 'Automatic', 870000000, 
            'Honda Civic 1.5L VTEC Turbo bản RS thể thao cá tính, trang bị hệ thống an toàn Honda SENSING tiên tiến.', 
            'https://images.unsplash.com/photo-1606016159991-dfe4f2746ad5?auto=format&fit=crop&w=600&q=80', 'Available', GETDATE());

-- Honda CR-V (Model: "Honda CR-V" in HF)
IF NOT EXISTS (SELECT 1 FROM Cars WHERE CarName = 'Honda CR-V L Turbo')
    INSERT INTO Cars (BrandId, CarName, Model, [Year], Color, Mileage, FuelType, Transmission, Price, Description, ImageUrl, Status, CreatedAt)
    VALUES (@HondaId, 'Honda CR-V L Turbo', 'CR-V', 2021, 'Gray', 22000, 'Gasoline', 'Automatic', 980000000, 
            'SUV 7 chỗ rộng rãi tiện nghi cho gia đình. Bảo dưỡng đầy đủ chính hãng Honda, cam kết không đâm đụng ngập nước.', 
            'https://images.unsplash.com/photo-1568605117036-5fe5e7bab0b7?auto=format&fit=crop&w=600&q=80', 'Available', GETDATE());

-- Ford Mustang (Model: "Ford Mustang" in HF)
IF NOT EXISTS (SELECT 1 FROM Cars WHERE CarName = 'Ford Mustang Ecoboost 2.3L')
    INSERT INTO Cars (BrandId, CarName, Model, [Year], Color, Mileage, FuelType, Transmission, Price, Description, ImageUrl, Status, CreatedAt)
    VALUES (@FordId, 'Ford Mustang Ecoboost 2.3L', 'Mustang', 2020, 'Yellow', 30000, 'Gasoline', 'Automatic', 1850000000, 
            'Mẫu xe cơ bắp Mỹ huyền thoại Ford Mustang động cơ EcoBoost 2.3L mạnh mẽ và tiết kiệm. Ngoại hình thể thao bắt mắt.', 
            'https://images.unsplash.com/photo-1611245801314-e0cf5bf9228d?auto=format&fit=crop&w=600&q=80', 'Available', GETDATE());

-- Audi A4 (Model: "Audi A4" in HF)
IF NOT EXISTS (SELECT 1 FROM Cars WHERE CarName = 'Audi A4 40 TFSI Advance')
    INSERT INTO Cars (BrandId, CarName, Model, [Year], Color, Mileage, FuelType, Transmission, Price, Description, ImageUrl, Status, CreatedAt)
    VALUES (@AudiId, 'Audi A4 40 TFSI Advance', 'A4', 2021, 'White', 20000, 'Gasoline', 'Automatic', 1450000000, 
            'Audi A4 kiểu dáng thanh lịch sang trọng phong cách châu Âu. Nội thất ốp gỗ cao cấp, hệ thống đèn LED Matrix đặc trưng.', 
            'https://images.unsplash.com/photo-1614162692292-7ac56d7f7f1e?auto=format&fit=crop&w=600&q=80', 'Available', GETDATE());

-- Mercedes-Benz C Class (Model: "Mercedes-Benz C Class" in HF)
IF NOT EXISTS (SELECT 1 FROM Cars WHERE CarName = 'Mercedes-Benz C200 Avantgarde Plus')
    INSERT INTO Cars (BrandId, CarName, Model, [Year], Color, Mileage, FuelType, Transmission, Price, Description, ImageUrl, Status, CreatedAt)
    VALUES (@MercId, 'Mercedes-Benz C200 Avantgarde Plus', 'C Class', 2022, 'Black', 15000, 'Gasoline', 'Automatic', 1580000000, 
            'Mercedes-Benz C Class mới, thiết kế sang trọng thừa hưởng từ dòng S-Class. Vận hành mượt mà, nhiều tiện nghi giải trí thông minh.', 
            'https://images.unsplash.com/photo-1618843479313-40f8afb4b4d8?auto=format&fit=crop&w=600&q=80', 'Available', GETDATE());

-- BMW 3-Series (Model: "BMW 3-Series" in HF)
IF NOT EXISTS (SELECT 1 FROM Cars WHERE CarName = 'BMW 330i M Sport')
    INSERT INTO Cars (BrandId, CarName, Model, [Year], Color, Mileage, FuelType, Transmission, Price, Description, ImageUrl, Status, CreatedAt)
    VALUES (@BmwId, 'BMW 330i M Sport', '3 Series', 2022, 'Blue', 11000, 'Gasoline', 'Automatic', 1890000000, 
            'BMW 3-Series bản 330i trang bị bodykit M Sport thể thao cá tính, động cơ 258 mã lực cực bốc, cảm giác lái đỉnh cao nhất phân khúc.', 
            'https://images.unsplash.com/photo-1555215695-3004980ad54e?auto=format&fit=crop&w=600&q=80', 'Available', GETDATE());

-- Porsche 911 (Model: "Porsche 911" in HF)
IF NOT EXISTS (SELECT 1 FROM Cars WHERE CarName = 'Porsche 911 Carrera S')
    INSERT INTO Cars (BrandId, CarName, Model, [Year], Color, Mileage, FuelType, Transmission, Price, Description, ImageUrl, Status, CreatedAt)
    VALUES (@PorscheId, 'Porsche 911 Carrera S', '911', 2021, 'Yellow', 6000, 'Gasoline', 'Automatic', 7800000000, 
            'Siêu xe thể thao Porsche 911 Carrera S (992) màu vàng Racing cực đẹp. Động cơ tăng áp kép Boxer 3.0L, full option sang trọng.', 
            'https://images.unsplash.com/photo-1503376780353-7e6692767b70?auto=format&fit=crop&w=600&q=80', 'Available', GETDATE());

-- Mazda CX-5 (Model: "Mazda CX-5" in HF)
IF NOT EXISTS (SELECT 1 FROM Cars WHERE CarName = 'Mazda CX-5 2.5 Signature Premium')
    INSERT INTO Cars (BrandId, CarName, Model, [Year], Color, Mileage, FuelType, Transmission, Price, Description, ImageUrl, Status, CreatedAt)
    VALUES (@MazdaId, 'Mazda CX-5 2.5 Signature Premium', 'CX-5', 2021, 'Red', 25000, 'Gasoline', 'Automatic', 820000000, 
            'Mazda CX-5 bản cao cấp nhất động cơ 2.5L dẫn động 2 cầu AWD, trang bị loa Bose cao cấp, màn hình HUD, gói an toàn i-Activsense.', 
            'https://images.unsplash.com/photo-1511919884226-fd3cad34687c?auto=format&fit=crop&w=600&q=80', 'Available', GETDATE());

-- Hyundai Tucson (Model: "Hyundai Tucson" in HF)
IF NOT EXISTS (SELECT 1 FROM Cars WHERE CarName = 'Hyundai Tucson 1.6 Turbo')
    INSERT INTO Cars (BrandId, CarName, Model, [Year], Color, Mileage, FuelType, Transmission, Price, Description, ImageUrl, Status, CreatedAt)
    VALUES (@HyundaiId, 'Hyundai Tucson 1.6 Turbo', 'Tucson', 2022, 'White', 18000, 'Gasoline', 'Automatic', 890000000, 
            'Hyundai Tucson thiết kế Sensuous Sportiness cực kỳ tương lai, bản 1.6 Turbo mạnh mẽ, vận hành êm ái, đầy ắp option.', 
            'https://images.unsplash.com/photo-1619767887304-4c57c46a6f1c?auto=format&fit=crop&w=600&q=80', 'Available', GETDATE());

-- Chevrolet Camaro (Model: "Chevrolet Camaro" in HF)
IF NOT EXISTS (SELECT 1 FROM Cars WHERE CarName = 'Chevrolet Camaro SS v8')
    INSERT INTO Cars (BrandId, CarName, Model, [Year], Color, Mileage, FuelType, Transmission, Price, Description, ImageUrl, Status, CreatedAt)
    VALUES (@ChevyId, 'Chevrolet Camaro SS v8', 'Camaro', 2019, 'Yellow', 29000, 'Gasoline', 'Automatic', 2600000000, 
            'Xe thể thao cơ bắp Chevrolet Camaro bản SS động cơ V8 6.2L cực khủng, âm thanh pô gầm rú mạnh mẽ, màu vàng Bumblebee.', 
            'https://images.unsplash.com/photo-1552519507-da3b142c6e3d?auto=format&fit=crop&w=600&q=80', 'Available', GETDATE());
GO

PRINT 'Car Model seeds completed successfully!';
