CREATE TABLE [AppRoles] (
    [RoleId] int NOT NULL IDENTITY,
    [RoleName] nvarchar(50) NOT NULL,
    CONSTRAINT [PK__AppRoles__8AFACE1ABED980DF] PRIMARY KEY ([RoleId])
);
GO


CREATE TABLE [CarBrands] (
    [BrandId] int NOT NULL IDENTITY,
    [BrandName] nvarchar(100) NOT NULL,
    [Country] nvarchar(100) NULL,
    [Description] nvarchar(500) NULL,
    CONSTRAINT [PK__CarBrand__DAD4F05EFE11BDE9] PRIMARY KEY ([BrandId])
);
GO


CREATE TABLE [MaintenancePackages] (
    [PackageId] int NOT NULL IDENTITY,
    [PackageName] nvarchar(150) NOT NULL,
    [Description] nvarchar(1000) NULL,
    [PackagePrice] decimal(18,2) NOT NULL,
    [Status] nvarchar(50) NOT NULL DEFAULT N'Available',
    [CreatedAt] datetime NOT NULL DEFAULT ((getdate())),
    [UpdatedAt] datetime NULL,
    CONSTRAINT [PK_MaintenancePackages] PRIMARY KEY ([PackageId])
);
GO


CREATE TABLE [PartCategories] (
    [CategoryId] int NOT NULL IDENTITY,
    [CategoryName] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NULL,
    CONSTRAINT [PK__PartCate__19093A0B38EB3018] PRIMARY KEY ([CategoryId])
);
GO


CREATE TABLE [Services] (
    [ServiceId] int NOT NULL IDENTITY,
    [ServiceName] nvarchar(150) NOT NULL,
    [Description] nvarchar(1000) NULL,
    [BasePrice] decimal(18,2) NOT NULL,
    [EstimatedDurationMinutes] int NOT NULL DEFAULT 30,
    [Status] nvarchar(50) NOT NULL DEFAULT N'Available',
    [CreatedAt] datetime NOT NULL DEFAULT ((getdate())),
    [UpdatedAt] datetime NULL,
    CONSTRAINT [PK_Services] PRIMARY KEY ([ServiceId])
);
GO


CREATE TABLE [AppUsers] (
    [UserId] int NOT NULL IDENTITY,
    [FullName] nvarchar(100) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [PasswordHash] nvarchar(255) NOT NULL,
    [PhoneNumber] nvarchar(20) NULL,
    [Address] nvarchar(255) NULL,
    [RoleId] int NOT NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedAt] datetime NOT NULL DEFAULT ((getdate())),
    [VerificationCode] nvarchar(max) NULL,
    [CodeExpiryTime] datetime2 NULL,
    CONSTRAINT [PK__AppUsers__1788CC4CD3D16938] PRIMARY KEY ([UserId]),
    CONSTRAINT [FK_AppUsers_AppRoles] FOREIGN KEY ([RoleId]) REFERENCES [AppRoles] ([RoleId])
);
GO


CREATE TABLE [Cars] (
    [CarId] int NOT NULL IDENTITY,
    [BrandId] int NOT NULL,
    [CarName] nvarchar(150) NOT NULL,
    [Model] nvarchar(100) NULL,
    [Year] int NOT NULL,
    [Color] nvarchar(50) NULL,
    [Mileage] int NOT NULL,
    [FuelType] nvarchar(50) NOT NULL,
    [Transmission] nvarchar(50) NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [Description] nvarchar(1000) NULL,
    [ImageUrl] nvarchar(500) NULL,
    [Status] nvarchar(50) NOT NULL DEFAULT N'Available',
    [CreatedAt] datetime NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__Cars__68A0342E9C46E9E9] PRIMARY KEY ([CarId]),
    CONSTRAINT [FK_Cars_CarBrands] FOREIGN KEY ([BrandId]) REFERENCES [CarBrands] ([BrandId])
);
GO


CREATE TABLE [Parts] (
    [PartId] int NOT NULL IDENTITY,
    [CategoryId] int NOT NULL,
    [PartName] nvarchar(150) NOT NULL,
    [PartCode] nvarchar(50) NOT NULL,
    [Brand] nvarchar(100) NULL,
    [Price] decimal(18,2) NOT NULL,
    [Quantity] int NOT NULL,
    [Description] nvarchar(1000) NULL,
    [ImageUrl] nvarchar(500) NULL,
    [Status] nvarchar(50) NOT NULL DEFAULT N'Available',
    [CreatedAt] datetime NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__Parts__7C3F0D509A440590] PRIMARY KEY ([PartId]),
    CONSTRAINT [FK_Parts_PartCategories] FOREIGN KEY ([CategoryId]) REFERENCES [PartCategories] ([CategoryId])
);
GO


CREATE TABLE [PackageServices] (
    [PackageId] int NOT NULL,
    [ServiceId] int NOT NULL,
    [Notes] nvarchar(255) NULL,
    [CreatedAt] datetime NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK_PackageServices] PRIMARY KEY ([PackageId], [ServiceId]),
    CONSTRAINT [FK_PackageServices_Packages] FOREIGN KEY ([PackageId]) REFERENCES [MaintenancePackages] ([PackageId]) ON DELETE CASCADE,
    CONSTRAINT [FK_PackageServices_Services] FOREIGN KEY ([ServiceId]) REFERENCES [Services] ([ServiceId])
);
GO


CREATE TABLE [ComboOrders] (
    [ComboOrderId] int NOT NULL IDENTITY,
    [CustomerId] int NOT NULL,
    [CustomerName] nvarchar(100) NOT NULL,
    [CustomerPhone] nvarchar(20) NOT NULL,
    [CustomerEmail] nvarchar(100) NULL,
    [ShippingAddress] nvarchar(255) NULL,
    [TotalAmount] decimal(18,2) NOT NULL DEFAULT 0.0,
    [Note] nvarchar(1000) NULL,
    [Source] nvarchar(50) NOT NULL DEFAULT N'manual',
    [ChatSessionId] nvarchar(100) NULL,
    [PurchaseType] nvarchar(20) NOT NULL DEFAULT N'Buyout',
    [Status] nvarchar(50) NOT NULL DEFAULT N'Pending',
    [DepositAmount] decimal(18,2) NULL,
    [DepositExpiresAt] datetime NULL,
    [CaptchaCode] nvarchar(20) NULL,
    [CaptchaGeneratedAt] datetime NULL,
    [IsCaptchaUsed] bit NOT NULL DEFAULT CAST(0 AS bit),
    [CaptchaUsedAt] datetime NULL,
    [FinalCaptchaCode] nvarchar(20) NULL,
    [FinalCaptchaGeneratedAt] datetime NULL,
    [IsFinalCaptchaUsed] bit NOT NULL DEFAULT CAST(0 AS bit),
    [FinalCaptchaUsedAt] datetime NULL,
    [CreatedAt] datetime NOT NULL DEFAULT ((getdate())),
    [UpdatedAt] datetime NULL,
    CONSTRAINT [PK__ComboOrders] PRIMARY KEY ([ComboOrderId]),
    CONSTRAINT [FK_ComboOrders_AppUsers] FOREIGN KEY ([CustomerId]) REFERENCES [AppUsers] ([UserId])
);
GO


CREATE TABLE [CustomerCars] (
    [CustomerCarId] int NOT NULL IDENTITY,
    [CustomerId] int NOT NULL,
    [BrandId] int NOT NULL,
    [Model] nvarchar(100) NOT NULL,
    [Year] int NULL,
    [VIN] nvarchar(50) NULL,
    [LicensePlate] nvarchar(30) NOT NULL,
    [Color] nvarchar(50) NULL,
    [ExpiredAt] datetime NULL,
    [CreatedAt] datetime NOT NULL DEFAULT ((getdate())),
    [UpdatedAt] datetime NULL,
    CONSTRAINT [PK_CustomerCars] PRIMARY KEY ([CustomerCarId]),
    CONSTRAINT [FK_CustomerCars_AppUsers] FOREIGN KEY ([CustomerId]) REFERENCES [AppUsers] ([UserId]),
    CONSTRAINT [FK_CustomerCars_CarBrands] FOREIGN KEY ([BrandId]) REFERENCES [CarBrands] ([BrandId])
);
GO


CREATE TABLE [PartOrders] (
    [OrderId] int NOT NULL IDENTITY,
    [CustomerId] int NOT NULL,
    [CustomerName] nvarchar(100) NOT NULL,
    [CustomerPhone] nvarchar(20) NOT NULL,
    [CustomerEmail] nvarchar(100) NULL,
    [ShippingAddress] nvarchar(255) NULL,
    [DeliveryMethod] nvarchar(50) NOT NULL DEFAULT N'Pickup',
    [ShippingFee] decimal(18,2) NOT NULL DEFAULT 0.0,
    [TotalAmount] decimal(18,2) NOT NULL,
    [Status] nvarchar(50) NOT NULL DEFAULT N'Pending',
    [CreatedAt] datetime NOT NULL DEFAULT ((getdate())),
    [UpdatedAt] datetime NULL,
    CONSTRAINT [PK__PartOrde__C3905BCF6A12AD11] PRIMARY KEY ([OrderId]),
    CONSTRAINT [FK_PartOrders_AppUsers] FOREIGN KEY ([CustomerId]) REFERENCES [AppUsers] ([UserId])
);
GO


CREATE TABLE [DepositCaptchas] (
    [CaptchaId] int NOT NULL IDENTITY,
    [Code] nvarchar(20) NOT NULL,
    [CarId] int NOT NULL,
    [IsUsed] bit NOT NULL,
    [CreatedAt] datetime NOT NULL DEFAULT ((getdate())),
    [UsedAt] datetime NULL,
    CONSTRAINT [PK_DepositCaptchas] PRIMARY KEY ([CaptchaId]),
    CONSTRAINT [FK_DepositCaptchas_Cars] FOREIGN KEY ([CarId]) REFERENCES [Cars] ([CarId]) ON DELETE CASCADE
);
GO


CREATE TABLE [PurchaseRequests] (
    [RequestId] int NOT NULL IDENTITY,
    [CarId] int NOT NULL,
    [CustomerId] int NOT NULL,
    [CustomerName] nvarchar(100) NOT NULL,
    [CustomerPhone] nvarchar(20) NOT NULL,
    [CustomerEmail] nvarchar(100) NULL,
    [Message] nvarchar(1000) NULL,
    [Status] nvarchar(50) NOT NULL DEFAULT N'Pending',
    [CreatedAt] datetime NOT NULL DEFAULT ((getdate())),
    [UpdatedAt] datetime NULL,
    [DepositAmount] decimal(18,2) NULL,
    [DepositDate] datetime NULL,
    [DepositExpiry] datetime NULL,
    [CaptchaCode] nvarchar(20) NULL,
    CONSTRAINT [PK__Purchase__33A8517AB622A517] PRIMARY KEY ([RequestId]),
    CONSTRAINT [FK_PurchaseRequests_AppUsers] FOREIGN KEY ([CustomerId]) REFERENCES [AppUsers] ([UserId]),
    CONSTRAINT [FK_PurchaseRequests_Cars] FOREIGN KEY ([CarId]) REFERENCES [Cars] ([CarId])
);
GO


CREATE TABLE [ServiceRequiredParts] (
    [ServiceId] int NOT NULL,
    [PartId] int NOT NULL,
    [QuantityRequired] int NOT NULL DEFAULT 1,
    [CreatedAt] datetime NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK_ServiceRequiredParts] PRIMARY KEY ([ServiceId], [PartId]),
    CONSTRAINT [FK_ServiceRequiredParts_Parts] FOREIGN KEY ([PartId]) REFERENCES [Parts] ([PartId]),
    CONSTRAINT [FK_ServiceRequiredParts_Services] FOREIGN KEY ([ServiceId]) REFERENCES [Services] ([ServiceId]) ON DELETE CASCADE
);
GO


CREATE TABLE [ComboOrderItems] (
    [ItemId] int NOT NULL IDENTITY,
    [ComboOrderId] int NOT NULL,
    [ItemType] nvarchar(20) NOT NULL,
    [ReferenceId] int NOT NULL,
    [ItemName] nvarchar(200) NOT NULL,
    [Quantity] int NOT NULL,
    [UnitPrice] decimal(18,2) NOT NULL,
    [SubTotal] decimal(18,2) NOT NULL,
    CONSTRAINT [PK__ComboOrderItems] PRIMARY KEY ([ItemId]),
    CONSTRAINT [FK_ComboOrderItems_ComboOrders] FOREIGN KEY ([ComboOrderId]) REFERENCES [ComboOrders] ([ComboOrderId]) ON DELETE CASCADE
);
GO


CREATE TABLE [MaintenanceAppointments] (
    [AppointmentId] int NOT NULL IDENTITY,
    [MasterInvoiceId] int NULL,
    [CustomerId] int NOT NULL,
    [CustomerCarId] int NOT NULL,
    [CustomerName] nvarchar(100) NOT NULL,
    [CustomerPhone] nvarchar(20) NOT NULL,
    [CustomerEmail] nvarchar(100) NULL,
    [AppointmentDate] date NOT NULL,
    [AppointmentTime] time NOT NULL,
    [Note] nvarchar(1000) NULL,
    [Status] nvarchar(50) NOT NULL DEFAULT N'Pending',
    [ExpiredAt] datetime NULL,
    [CreatedAt] datetime NOT NULL DEFAULT ((getdate())),
    [UpdatedAt] datetime NULL,
    CONSTRAINT [PK_MaintenanceAppointments] PRIMARY KEY ([AppointmentId]),
    CONSTRAINT [FK_MaintenanceAppointments_AppUsers] FOREIGN KEY ([CustomerId]) REFERENCES [AppUsers] ([UserId]),
    CONSTRAINT [FK_MaintenanceAppointments_CustomerCars] FOREIGN KEY ([CustomerCarId]) REFERENCES [CustomerCars] ([CustomerCarId])
);
GO


CREATE TABLE [PartOrderDetails] (
    [OrderDetailId] int NOT NULL IDENTITY,
    [OrderId] int NOT NULL,
    [PartId] int NOT NULL,
    [Quantity] int NOT NULL,
    [UnitPrice] decimal(18,2) NOT NULL,
    [SubTotal] decimal(18,2) NOT NULL,
    CONSTRAINT [PK__PartOrde__D3B9D36CC532A696] PRIMARY KEY ([OrderDetailId]),
    CONSTRAINT [FK_PartOrderDetails_PartOrders] FOREIGN KEY ([OrderId]) REFERENCES [PartOrders] ([OrderId]),
    CONSTRAINT [FK_PartOrderDetails_Parts] FOREIGN KEY ([PartId]) REFERENCES [Parts] ([PartId])
);
GO


CREATE TABLE [AppointmentDetails] (
    [AppointmentDetailId] int NOT NULL IDENTITY,
    [AppointmentId] int NOT NULL,
    [PackageId] int NULL,
    [ServiceId] int NULL,
    [UnitPrice] decimal(18,2) NOT NULL,
    [Quantity] int NOT NULL DEFAULT 1,
    [CreatedAt] datetime NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK_AppointmentDetails] PRIMARY KEY ([AppointmentDetailId]),
    CONSTRAINT [FK_AppointmentDetails_Appointments] FOREIGN KEY ([AppointmentId]) REFERENCES [MaintenanceAppointments] ([AppointmentId]) ON DELETE CASCADE,
    CONSTRAINT [FK_AppointmentDetails_Packages] FOREIGN KEY ([PackageId]) REFERENCES [MaintenancePackages] ([PackageId]),
    CONSTRAINT [FK_AppointmentDetails_Services] FOREIGN KEY ([ServiceId]) REFERENCES [Services] ([ServiceId])
);
GO


CREATE TABLE [AppointmentConsumedParts] (
    [ConsumedPartId] int NOT NULL IDENTITY,
    [AppointmentId] int NOT NULL,
    [AppointmentDetailId] int NULL,
    [PartId] int NOT NULL,
    [Quantity] int NOT NULL,
    [UnitPrice] decimal(18,2) NOT NULL,
    [IsIncurred] bit NOT NULL DEFAULT CAST(0 AS bit),
    [ApprovedByCustomer] bit NOT NULL DEFAULT CAST(1 AS bit),
    [Notes] nvarchar(500) NULL,
    [CreatedAt] datetime NOT NULL DEFAULT ((getdate())),
    [UpdatedAt] datetime NULL,
    CONSTRAINT [PK_AppointmentConsumedParts] PRIMARY KEY ([ConsumedPartId]),
    CONSTRAINT [FK_AppointmentConsumedParts_Appointments] FOREIGN KEY ([AppointmentId]) REFERENCES [MaintenanceAppointments] ([AppointmentId]) ON DELETE CASCADE,
    CONSTRAINT [FK_AppointmentConsumedParts_Details] FOREIGN KEY ([AppointmentDetailId]) REFERENCES [AppointmentDetails] ([AppointmentDetailId]),
    CONSTRAINT [FK_AppointmentConsumedParts_Parts] FOREIGN KEY ([PartId]) REFERENCES [Parts] ([PartId])
);
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'RoleName') AND [object_id] = OBJECT_ID(N'[AppRoles]'))
    SET IDENTITY_INSERT [AppRoles] ON;
INSERT INTO [AppRoles] ([RoleId], [RoleName])
VALUES (1, N'Admin'),
(2, N'Customer');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'RoleName') AND [object_id] = OBJECT_ID(N'[AppRoles]'))
    SET IDENTITY_INSERT [AppRoles] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'BrandId', N'BrandName', N'Country', N'Description') AND [object_id] = OBJECT_ID(N'[CarBrands]'))
    SET IDENTITY_INSERT [CarBrands] ON;
INSERT INTO [CarBrands] ([BrandId], [BrandName], [Country], [Description])
VALUES (1, N'Toyota', N'Japan', N'Toyota Motor Corporation'),
(2, N'Ford', N'USA', N'Ford Motor Company'),
(3, N'VinFast', N'Vietnam', N'VinFast Vietnam'),
(4, N'BMW', N'Germany', N'Bayerische Motoren Werke AG');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'BrandId', N'BrandName', N'Country', N'Description') AND [object_id] = OBJECT_ID(N'[CarBrands]'))
    SET IDENTITY_INSERT [CarBrands] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PackageId', N'CreatedAt', N'Description', N'PackageName', N'PackagePrice', N'Status', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[MaintenancePackages]'))
    SET IDENTITY_INSERT [MaintenancePackages] ON;
INSERT INTO [MaintenancePackages] ([PackageId], [CreatedAt], [Description], [PackageName], [PackagePrice], [Status], [UpdatedAt])
VALUES (1, '2025-01-01T00:00:00.000', N'Goi bao duong co ban giup xe van hanh tron tru bao gom thay dau, kiem tra phanh va ra soat loi.', N'Bao duong Dinh ky Tieu chuan 10.000km', 1200000.0, N'Available', NULL),
(2, '2025-01-01T00:00:00.000', N'Lam lanh sau, diet khuan dan lanh dieu hoa noi that.', N'Cham soc Dieu hoa VIP don he', 950000.0, N'Available', NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PackageId', N'CreatedAt', N'Description', N'PackageName', N'PackagePrice', N'Status', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[MaintenancePackages]'))
    SET IDENTITY_INSERT [MaintenancePackages] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CategoryId', N'CategoryName', N'Description') AND [object_id] = OBJECT_ID(N'[PartCategories]'))
    SET IDENTITY_INSERT [PartCategories] ON;
INSERT INTO [PartCategories] ([CategoryId], [CategoryName], [Description])
VALUES (1, N'D?ng co & Truy?n d?ng', N'C?c b? ph?n li?n quan d?n d?ng co, h?p s? v? truy?n d?ng.'),
(2, N'H? th?ng di?n & ?c quy', N'?c quy, m?y ph?t di?n, d?n v? h? th?ng di?n.'),
(3, N'D?u nh?t & H?a ch?t', N'D?u m?y, nu?c l?m m?t, d?u phanh v? h?a ch?t b?o du?ng.'),
(4, N'Ngo?i th?t & Ph? ki?n', N'L?p xe, g?t mua, guong v? c?c ph? ki?n trang tr? ngo?i th?t.');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CategoryId', N'CategoryName', N'Description') AND [object_id] = OBJECT_ID(N'[PartCategories]'))
    SET IDENTITY_INSERT [PartCategories] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ServiceId', N'BasePrice', N'CreatedAt', N'Description', N'EstimatedDurationMinutes', N'ServiceName', N'Status', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Services]'))
    SET IDENTITY_INSERT [Services] ON;
INSERT INTO [Services] ([ServiceId], [BasePrice], [CreatedAt], [Description], [EstimatedDurationMinutes], [ServiceName], [Status], [UpdatedAt])
VALUES (1, 200000.0, '2025-01-01T00:00:00.000', N'Xa dau cu, thay loc dau chinh hang, cham dau dong co Castrol moi phu hop.', 30, N'Thay dau dong co & Coc loc dau', N'Available', NULL),
(2, 450000.0, '2025-01-01T00:00:00.000', N'Su dung may quet laser 3D de can chinh do chum banh xe va can bang dong.', 45, N'Can chinh thuoc lai do chum lop', N'Available', NULL),
(3, 600000.0, '2025-01-01T00:00:00.000', N'Su dung may noi soi chuyen dung lam sach bui ban dan lanh khong can thao taplo.', 60, N'Ve sinh dan lanh dieu hoa noi that', N'Available', NULL),
(4, 150000.0, '2025-01-01T00:00:00.000', N'Kiem tra may gam, phanh, lop, dien than xe, nuoc lam mat, chan doan loi bang may chuyen dung.', 40, N'Kiem tra toan dien 30 hang muc ky thuat', N'Available', NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ServiceId', N'BasePrice', N'CreatedAt', N'Description', N'EstimatedDurationMinutes', N'ServiceName', N'Status', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Services]'))
    SET IDENTITY_INSERT [Services] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'Address', N'CodeExpiryTime', N'CreatedAt', N'Email', N'FullName', N'IsActive', N'PasswordHash', N'PhoneNumber', N'RoleId', N'VerificationCode') AND [object_id] = OBJECT_ID(N'[AppUsers]'))
    SET IDENTITY_INSERT [AppUsers] ON;
INSERT INTO [AppUsers] ([UserId], [Address], [CodeExpiryTime], [CreatedAt], [Email], [FullName], [IsActive], [PasswordHash], [PhoneNumber], [RoleId], [VerificationCode])
VALUES (1, N'Hanoi', NULL, '2025-01-01T00:00:00.000', N'admin@gmail.com', N'System Admin', CAST(1 AS bit), N'$2a$11$ivuFcskipHfVJyUk7X7Cy.72DYWJAKQhFt7uaF2kMrwZ/LAHW1cWO', N'0987654321', 1, NULL),
(2, N'HCM City', NULL, '2025-01-01T00:00:00.000', N'customer@gmail.com', N'John Customer', CAST(1 AS bit), N'$2a$11$iR0JU.l1mLeRCyKuClJFxuWqtweaw2kS3oZSRG/lAcD00M603P5Mm', N'0123456789', 2, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'Address', N'CodeExpiryTime', N'CreatedAt', N'Email', N'FullName', N'IsActive', N'PasswordHash', N'PhoneNumber', N'RoleId', N'VerificationCode') AND [object_id] = OBJECT_ID(N'[AppUsers]'))
    SET IDENTITY_INSERT [AppUsers] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CarId', N'BrandId', N'CarName', N'Color', N'CreatedAt', N'Description', N'FuelType', N'ImageUrl', N'Mileage', N'Model', N'Price', N'Status', N'Transmission', N'Year') AND [object_id] = OBJECT_ID(N'[Cars]'))
    SET IDENTITY_INSERT [Cars] ON;
INSERT INTO [Cars] ([CarId], [BrandId], [CarName], [Color], [CreatedAt], [Description], [FuelType], [ImageUrl], [Mileage], [Model], [Price], [Status], [Transmission], [Year])
VALUES (1, 1, N'Toyota Camry 2.5Q', N'Black', '2025-01-01T00:00:00.000', N'Xe sang tr?ng, l?ch l?m, gia d?nh s? d?ng k?, b?o du?ng ch?nh h?ng.', N'Gasoline', N'https://images.unsplash.com/photo-1621007947382-bb3c3994e3fb?auto=format&fit=crop&w=600&q=80', 15000, N'Camry', 1350000000.0, N'Available', N'Automatic', 2022),
(2, 1, N'Toyota Vios 1.5G', N'White', '2025-01-01T00:00:00.000', N'Xe qu?c d?n ti?t ki?m nhi?n li?u, v?n h?nh b?n b?.', N'Gasoline', N'https://images.unsplash.com/photo-1605559424843-9e4c228bf1c2?auto=format&fit=crop&w=600&q=80', 28000, N'Vios', 520000000.0, N'Available', N'Automatic', 2021),
(3, 2, N'Ford Ranger Wildtrak 2.0L', N'Orange', '2025-01-01T00:00:00.000', N'Vua b?n t?i, phi?n b?n cao c?p nh?t Wildtrak 2 c?u, d?y d? c?ng ngh?.', N'Diesel', N'https://images.unsplash.com/photo-1533473359331-0135ef1b58bf?auto=format&fit=crop&w=600&q=80', 8000, N'Ranger', 960000000.0, N'Available', N'Automatic', 2023),
(4, 3, N'VinFast VF8 Plus', N'Blue', '2025-01-01T00:00:00.000', N'Xe di?n th?ng minh Vi?t Nam, b?n Plus pin SDI, c?ng ngh? ADAS hi?n d?i.', N'Electric', N'https://images.unsplash.com/photo-1563720223185-11003d516935?auto=format&fit=crop&w=600&q=80', 5000, N'VF8', 1100000000.0, N'Available', N'Automatic', 2023),
(5, 4, N'BMW 320i Sport Line', N'Red', '2025-01-01T00:00:00.000', N'D?ng sedan th? thao l?i c?c hay, ngo?i h?nh tr? trung nang d?ng.', N'Gasoline', N'https://images.unsplash.com/photo-1555215695-3004980ad54e?auto=format&fit=crop&w=600&q=80', 35000, N'3 Series', 1250000000.0, N'Available', N'Automatic', 2020),
(6, 3, N'VinFast VF5 Plus', N'Gray', '2025-01-01T00:00:00.000', N'Xe d? th? c? nh? th?ng minh, c?c k? ti?t ki?m v? nh? g?n.', N'Electric', N'https://images.unsplash.com/photo-1617788138017-80ad40651399?auto=format&fit=crop&w=600&q=80', 2000, N'VF5', 450000000.0, N'Available', N'Automatic', 2023);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CarId', N'BrandId', N'CarName', N'Color', N'CreatedAt', N'Description', N'FuelType', N'ImageUrl', N'Mileage', N'Model', N'Price', N'Status', N'Transmission', N'Year') AND [object_id] = OBJECT_ID(N'[Cars]'))
    SET IDENTITY_INSERT [Cars] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PackageId', N'ServiceId', N'CreatedAt', N'Notes') AND [object_id] = OBJECT_ID(N'[PackageServices]'))
    SET IDENTITY_INSERT [PackageServices] ON;
INSERT INTO [PackageServices] ([PackageId], [ServiceId], [CreatedAt], [Notes])
VALUES (1, 1, '2025-01-01T00:00:00.000', NULL),
(1, 4, '2025-01-01T00:00:00.000', NULL),
(2, 3, '2025-01-01T00:00:00.000', NULL),
(2, 4, '2025-01-01T00:00:00.000', NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PackageId', N'ServiceId', N'CreatedAt', N'Notes') AND [object_id] = OBJECT_ID(N'[PackageServices]'))
    SET IDENTITY_INSERT [PackageServices] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PartId', N'Brand', N'CategoryId', N'CreatedAt', N'Description', N'ImageUrl', N'PartCode', N'PartName', N'Price', N'Quantity', N'Status') AND [object_id] = OBJECT_ID(N'[Parts]'))
    SET IDENTITY_INSERT [Parts] ON;
INSERT INTO [Parts] ([PartId], [Brand], [CategoryId], [CreatedAt], [Description], [ImageUrl], [PartCode], [PartName], [Price], [Quantity], [Status])
VALUES (1, N'Michelin', 4, '2025-01-01T00:00:00.000', N'L?p hi?u nang cao, b?m du?ng c?c t?t trong m?i di?u ki?n th?i ti?t.', N'https://images.unsplash.com/photo-1578844251758-2f71da64c96f?auto=format&fit=crop&w=600&q=80', N'PT-MIC-PS4', N'L?p xe Michelin Pilot Sport 4', 3200000.0, 40, N'Available'),
(2, N'GS Battery', 2, '2025-01-01T00:00:00.000', N'?c quy kh? mi?n b?o du?ng, d? b?n cao, kh?i d?ng m?nh m?.', N'https://images.unsplash.com/photo-1619642751034-765dfdf7c58e?auto=format&fit=crop&w=600&q=80', N'PT-GS-12V45', N'?c quy GS 12V 45Ah', 1450000.0, 25, N'Available'),
(3, N'Castrol', 3, '2025-01-01T00:00:00.000', N'D?u nh?t c?ng ngh? t?ng h?p ho?n to?n b?o v? d?ng co ngay khi kh?i d?ng.', N'https://images.unsplash.com/photo-1622560480605-d83c853bc5c3?auto=format&fit=crop&w=600&q=80', N'PT-CAS-5W30', N'D?u nh?t Castrol Magnatec 5W-30', 850000.0, 50, N'Available'),
(4, N'Bosch', 4, '2025-01-01T00:00:00.000', N'G?t mua cao c?p t? Bosch D?c, g?t s?ch nu?c nh? nh?ng, ?m ?i.', N'https://images.unsplash.com/photo-1517524206127-48bbd363f3d7?auto=format&fit=crop&w=600&q=80', N'PT-BOS-AERO', N'G?t mua Bosch Aerotwin', 450000.0, 60, N'Available'),
(5, N'Philips', 2, '2025-01-01T00:00:00.000', N'B?ng d?n LED H7 si?u s?ng, gom s?ng t?t, d? b?n l?n d?n 5 nam.', N'https://images.unsplash.com/photo-1508974239320-0a029497e820?auto=format&fit=crop&w=600&q=80', N'PT-PHI-LEDH7', N'D?n pha LED Philips Ultinon Essential', 1200000.0, 15, N'Available');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PartId', N'Brand', N'CategoryId', N'CreatedAt', N'Description', N'ImageUrl', N'PartCode', N'PartName', N'Price', N'Quantity', N'Status') AND [object_id] = OBJECT_ID(N'[Parts]'))
    SET IDENTITY_INSERT [Parts] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PartId', N'ServiceId', N'CreatedAt', N'QuantityRequired') AND [object_id] = OBJECT_ID(N'[ServiceRequiredParts]'))
    SET IDENTITY_INSERT [ServiceRequiredParts] ON;
INSERT INTO [ServiceRequiredParts] ([PartId], [ServiceId], [CreatedAt], [QuantityRequired])
VALUES (3, 1, '2025-01-01T00:00:00.000', 4);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PartId', N'ServiceId', N'CreatedAt', N'QuantityRequired') AND [object_id] = OBJECT_ID(N'[ServiceRequiredParts]'))
    SET IDENTITY_INSERT [ServiceRequiredParts] OFF;
GO


CREATE INDEX [IX_AppointmentConsumedParts_AppointmentDetailId] ON [AppointmentConsumedParts] ([AppointmentDetailId]);
GO


CREATE INDEX [IX_AppointmentConsumedParts_AppointmentId] ON [AppointmentConsumedParts] ([AppointmentId]);
GO


CREATE INDEX [IX_AppointmentConsumedParts_PartId] ON [AppointmentConsumedParts] ([PartId]);
GO


CREATE INDEX [IX_AppointmentDetails_AppointmentId] ON [AppointmentDetails] ([AppointmentId]);
GO


CREATE INDEX [IX_AppointmentDetails_PackageId] ON [AppointmentDetails] ([PackageId]);
GO


CREATE INDEX [IX_AppointmentDetails_ServiceId] ON [AppointmentDetails] ([ServiceId]);
GO


CREATE UNIQUE INDEX [UQ__AppRoles__8A2B6160E1219BDE] ON [AppRoles] ([RoleName]);
GO


CREATE INDEX [IX_AppUsers_RoleId] ON [AppUsers] ([RoleId]);
GO


CREATE UNIQUE INDEX [UQ__AppUsers__A9D10534CD643903] ON [AppUsers] ([Email]);
GO


CREATE INDEX [IX_Cars_BrandId] ON [Cars] ([BrandId]);
GO


CREATE INDEX [IX_ComboOrderItems_ComboOrderId] ON [ComboOrderItems] ([ComboOrderId]);
GO


CREATE INDEX [IX_ComboOrders_CustomerId] ON [ComboOrders] ([CustomerId]);
GO


CREATE INDEX [IX_CustomerCars_BrandId] ON [CustomerCars] ([BrandId]);
GO


CREATE INDEX [IX_CustomerCars_CustomerId] ON [CustomerCars] ([CustomerId]);
GO


CREATE UNIQUE INDEX [IX_CustomerCars_LicensePlate] ON [CustomerCars] ([LicensePlate]);
GO


CREATE UNIQUE INDEX [IX_CustomerCars_VIN] ON [CustomerCars] ([VIN]) WHERE [VIN] IS NOT NULL;
GO


CREATE INDEX [IX_DepositCaptchas_CarId] ON [DepositCaptchas] ([CarId]);
GO


CREATE UNIQUE INDEX [IX_DepositCaptchas_Code] ON [DepositCaptchas] ([Code]);
GO


CREATE INDEX [IX_MaintenanceAppointments_CustomerCarId] ON [MaintenanceAppointments] ([CustomerCarId]);
GO


CREATE INDEX [IX_MaintenanceAppointments_CustomerId] ON [MaintenanceAppointments] ([CustomerId]);
GO


CREATE INDEX [IX_PackageServices_ServiceId] ON [PackageServices] ([ServiceId]);
GO


CREATE INDEX [IX_PartOrderDetails_OrderId] ON [PartOrderDetails] ([OrderId]);
GO


CREATE INDEX [IX_PartOrderDetails_PartId] ON [PartOrderDetails] ([PartId]);
GO


CREATE INDEX [IX_PartOrders_CustomerId] ON [PartOrders] ([CustomerId]);
GO


CREATE INDEX [IX_Parts_CategoryId] ON [Parts] ([CategoryId]);
GO


CREATE UNIQUE INDEX [UQ__Parts__6525D39D6EAC6A52] ON [Parts] ([PartCode]);
GO


CREATE INDEX [IX_PurchaseRequests_CarId] ON [PurchaseRequests] ([CarId]);
GO


CREATE INDEX [IX_PurchaseRequests_CustomerId] ON [PurchaseRequests] ([CustomerId]);
GO


CREATE INDEX [IX_ServiceRequiredParts_PartId] ON [ServiceRequiredParts] ([PartId]);
GO


