CREATE DATABASE EcartDB
Use EcartDB
GO
--DROP Tables IF EXIST
IF OBJECT_ID('Roles') IS NOT NULL
    DROP TABLE Roles

IF OBJECT_ID('Users') IS NOT NULL
    DROP TABLE Users

IF OBJECT_ID('Categories') IS NOT NULL
    DROP TABLE Categories

IF OBJECT_ID('Products') IS NOT NULL
    DROP TABLE Products

IF OBJECT_ID('PurchaseDetails') IS NOT NULL
    DROP TABLE PurchaseDetails

IF OBJECT_ID('CardDetails') IS NOT NULL
    DROP TABLE CardDetails

--DROP FUNCTIONS

IF OBJECT_ID('Ufn_FetchCategories') IS NOT NULL
    DROP FUNCTION Ufn_FetchCategories

IF OBJECT_ID('ufn_FetchProductDetails') IS NOT NULL
    DROP FUNCTION ufn_FetchProductDetails

--DROP STORED PROCEEDURE
IF OBJECT_ID('usp_AddPurchaseDetails') IS NOT NULL
    DROP PROC usp_AddPurchaseDetails

IF OBJECT_ID('usp_UpdateCardBalance') IS NOT NULL
    DROP PROC usp_UpdateCardBalance

IF OBJECT_ID('usp_RegisterNewUser') IS NOT NULL
    DROP PROC usp_RegisterNewUser

IF OBJECT_ID('usp_AddNewProduct') IS NOT NULL
    DROP PROC usp_AddNewProduct

IF OBJECT_ID('usp_AddCardDetails') IS NOT NULL
    DROP PROC usp_AddCardDetails
GO

--TABLES
CREATE TABLE [Roles]
(
	[RoleId] TINYINT CONSTRAINT pk_RoleId PRIMARY KEY IDENTITY,
	[RoleName] VARCHAR(20) CONSTRAINT uq_RoleName UNIQUE
)
GO 

Create Table [Users](
[EmailId] VARCHAR(50) CONSTRAINT pk_EmailId PRIMARY KEY,
[FirstName] VARCHAR(20) NULL,
[LastName] VARCHAR(20) NULL,
[UserPassword] VARCHAR(15) NOT NULL,
[RoleId] TINYINT CONSTRAINT fk_RoleId REFERENCES Roles(RoleId),
[PhoneNumber] VARCHAR(10) CONSTRAINT chk_Customers_PhoneNumbe CHECK(LEN(PhoneNumber)=10) NOT NULL,
[AddressLine] VARCHAR(50) NOT NULL,
[City] VARCHAR(50) NOT NULL,
[State] VARCHAR(50) NOT NULL,
[Country] VARCHAR(50) NOT NULL,
[ZipCode] VARCHAR(6) NOT NULL
)
GO

CREATE TABLE Categories
(
	[CategoryId] TINYINT CONSTRAINT pk_CategoryId PRIMARY KEY IDENTITY,
	[CategoryName] VARCHAR(20) CONSTRAINT uq_CategoryName UNIQUE NOT NULL 
)
GO

CREATE TABLE Products
(
	[ProductId] CHAR(4) CONSTRAINT pk_ProductId PRIMARY KEY CONSTRAINT chk_ProductId CHECK(ProductId LIKE 'P%'),
	[ProductName] VARCHAR(50) CONSTRAINT uq_ProductName UNIQUE NOT NULL,
	[CategoryId] TINYINT CONSTRAINT fk_CategoryId REFERENCES Categories(CategoryId),
	[Price] NUMERIC(8) CONSTRAINT chk_Price CHECK(Price>0) NOT NULL,
	[QuantityAvailable] INT CONSTRAINT chk_QuantityAvailable CHECK (QuantityAvailable>=0) NOT NULL
)
GO

CREATE TABLE PurchaseDetails
(
	[PurchaseId] BIGINT CONSTRAINT pk_PurchaseId PRIMARY KEY IDENTITY(1000,1),
	[EmailId] VARCHAR(50) CONSTRAINT fk_EmailId REFERENCES Users(EmailId),
	[ProductId] CHAR(4) CONSTRAINT fk_ProductId REFERENCES Products(ProductId),
	[ProductName] VARCHAR(50),
	[QuantityPurchased] SMALLINT CONSTRAINT chk_QuantityPurchased CHECK(QuantityPurchased>0) NOT NULL,
	[DateOfPurchase] DATETIME CONSTRAINT chk_DateOfPurchase CHECK(DateOfPurchase<=GETDATE()) DEFAULT GETDATE() NOT NULL,
)
SET IDENTITY_INSERT PURCHASEDETAILS OFF
GO

CREATE TABLE CardDetails
(
	[CardNumber] VARCHAR(16) CONSTRAINT pk_CardNumber PRIMARY KEY,
	[NameOnCard] VARCHAR(40) NOT NULL,
	[EmailId] VARCHAR(50) CONSTRAINT fkey_EmailId REFERENCES Users(EmailId),
	[CardType] VARCHAR(18) NOT NULL CONSTRAINT chk_CardType CHECK (CardType IN ('AMERICAN EXPRESS','MASTERCARD','VISA','DISCOVER')),
	[CVVNumber] NUMERIC(3) NOT NULL,
	[ExpiryDate] DATE NOT NULL CONSTRAINT chk_ExpiryDate CHECK(ExpiryDate>=CAST(GETDATE() AS DATE)),
	[Balance] DECIMAL(10,2) CONSTRAINT chk_Balance CHECK([Balance]>=0) DEFAULT 2000000
)
GO

CREATE TABLE ShoppingCart
(
	[ProductId] CHAR(4) PRIMARY KEY NOT NULL,
	[ProductName] VARCHAR(50) NOT NULL,
	[CategoryId] TINYINT,
	[Price] NUMERIC(8) CHECK(Price>0) NOT NULL,
	[Quantity] INT CHECK (Quantity >=0) NOT NULL DEFAULT 1,
	[PurchaseDate] DATETIME NOT NULL DEFAULT GETDATE()
)
GO

--INDEXES
CREATE INDEX ix_RoleId ON Users(RoleId)
CREATE INDEX ix_CategoryId ON Products(CategoryId)
CREATE INDEX ix_EmailId ON PurchaseDetails(EmailId)
CREATE INDEX ix_ProductId ON PurchaseDetails(ProductId)
GO

--FUNCTIONS
CREATE FUNCTION ufn_FetchProductDetails(@CategoryId INT)
RETURNS TABLE 
AS
RETURN (SELECT ProductId,ProductName,Price,QuantityAvailable FROM Products WHERE CategoryId=@CategoryId)
GO

CREATE FUNCTION ufn_FecthCardDetails(@CardNumber NUMERIC(16))
RETURNS TABLE 
AS
	RETURN (SELECT NameOnCard,CardType,CVVNumber,ExpiryDate,Balance FROM CardDetails WHERE CardNumber=@CardNumber)
GO

CREATE FUNCTION ufn_GetShoppingCart()
RETURNS TABLE
AS
RETURN (SELECT * FROM [ShoppingCart])
GO

--STORED PROCEEDURES

CREATE PROCEDURE usp_AddCardDetails
(
@CardNumber VARCHAR(16),
@NameOnCard VARCHAR(40),
@CardType VARCHAR(18), 
@CVVNumber NUMERIC(3),
@ExpiryDate DATE,
@Balance DECIMAL(10,2)
)
AS
BEGIN
	DECLARE @retval INT
		BEGIN TRY
			IF (@NameOnCard IS NULL)
				SET @retval = -1
			ELSE IF (@CardType IS NULL)
				SET @retval = -2
			ELSE IF(LEN(@CardNumber) <> 16 OR @CardNumber IS NULL)
				SET @retval = -3
			ELSE IF(@ExpiryDate IS NULL)
				SET @retval = -4
			ELSE
				BEGIN
					INSERT INTO CardDetails VALUES (@CardNumber, @NameOnCard, @CardType, @CVVNumber, CAST(@ExpiryDate AS DATE), @Balance)				
					SET @retval = 1
				END 
		END TRY
		BEGIN CATCH		
			SET @retval = -99
		END CATCH
	RETURN @retval
	END
GO

CREATE PROCEDURE usp_AddPurchaseDetails
(
	@EmailId VARCHAR(50),
	@ProductId CHAR(4),
	@QuantityPurchased INT,
	@PurchaseId BIGINT OUTPUT
)
AS
BEGIN
	DECLARE @retval int
	SET @PurchaseId=0	
		BEGIN TRY
			IF (@EmailId IS NULL)
				SET @retval = -1
			ELSE IF NOT EXISTS (SELECT @EmailId FROM Users WHERE EmailId=@EmailId)
				SET @retval = -2
			ELSE IF (@ProductId IS NULL)
				SET @retval = -3
			ELSE IF NOT EXISTS (SELECT ProductId FROM Products WHERE ProductId=@ProductId)
				SET @retval = -4
			ELSE IF ((@QuantityPurchased<=0) OR (@QuantityPurchased IS NULL))
				SET @retval = -5
			ELSE
				BEGIN
					INSERT INTO PurchaseDetails VALUES (@EmailId, @ProductId, @QuantityPurchased, DEFAULT)
					SELECT @PurchaseId=IDENT_CURRENT('PurchaseDetails')
					SET @retval = 1
				END
		END TRY
		BEGIN CATCH
			SET @PurchaseId=0			
			SET @retval = -99
		END CATCH
		RETURN @retval
	END
GO

CREATE PROCEDURE usp_UpdateCardBalance
(
	@CardNumber NUMERIC(16),
	@NameOnCard VARCHAR(40),
	@CardType CHAR(6),
	@CVVNumber NUMERIC(3),
	@ExpiryDate DATE,
	@Price DECIMAL(8)
)
AS
BEGIN
	DECLARE @TempUserName VARCHAR(40), @TempCardType CHAR(6), @TempCVVNumber NUMERIC(3), @TempExpiryDate DATE, @Balance DECIMAL(8), @retval int
	BEGIN TRY
		IF (@CardNumber IS NULL)
			SET @retval = -1
		ELSE IF NOT EXISTS(SELECT * FROM CardDetails WHERE CardNumber=@CardNumber)
			SET @retval = -2
		ELSE
			BEGIN
				SELECT @TempUserName=NameOnCard, @TempCardType=CardType, @TempCVVNumber=CVVNumber, @TempExpiryDate=ExpiryDate, @Balance=Balance 
				FROM CardDetails 
				WHERE CardNumber=@CardNumber
				IF ((@TempUserName<>@NameOnCard) OR (@NameOnCard IS NULL))
					SET @retval = -3
				ELSE IF ((@TempCardType<>@CardType) OR (@CardType IS NULL))
					SET @retval = -4
				ELSE IF ((@TempCVVNumber<>@CVVNumber) OR (@CVVNumber IS NULL))
					SET @retval = -5			
				ELSE IF ((@TempExpiryDate<>@ExpiryDate) OR (@ExpiryDate IS NULL))
					SET @retval = -6
				ELSE IF ((@Balance<@Price) OR (@Price IS NULL))
					SET @retval = -7
				ELSE
					BEGIN
						UPDATE Carddetails SET Balance=Balance-@Price WHERE CardNumber=@CardNumber
						SET @retval = 1
					END
			END
		SELECT @retval 
	END TRY
	BEGIN CATCH
		SET @retval = -99
		SELECT @retval 
	END CATCH
END
GO

CREATE PROCEDURE usp_RegisterNewUser
(
	@UserPassword VARCHAR(15),
	@Gender CHAR,
	@EmailId VARCHAR(50),
	@DateOfBirth DATE,
	@Address VARCHAR(200)
)
AS
BEGIN
	DECLARE @RoleId TINYINT,
		@retval int
	BEGIN TRY
		IF (LEN(@EmailId)<4 OR LEN(@EmailId)>50 OR (@EmailId IS NULL))
			SET @retval = -1
		ELSE IF (LEN(@UserPassword)<8 OR LEN(@UserPassword)>15 OR (@UserPassword IS NULL))
			SET @retval = -2
		ELSE IF (@Gender<>'F' AND @Gender<>'M' OR (@Gender Is NULL))
			SET @retval = -3		
		ELSE IF (@DateOfBirth>=CAST(GETDATE() AS DATE) OR (@DateOfBirth IS NULL))
			SET @retval = -4
		ELSE IF DATEDIFF(d,@DateOfBirth,GETDATE())<6570
			SET @retval = -5
		ELSE IF (@Address IS NULL)
			SET @retval = -6
		ELSE
			BEGIN
				SELECT @RoleId=RoleId FROM Roles WHERE RoleName='Customer'
				INSERT INTO Users VALUES 
				(@EmailId,@UserPassword, @RoleId, @Gender, @DateOfBirth, @Address)
				SET @retval = 1			
			END
		SELECT @retval 
		END TRY
		BEGIN CATCH
			SET @retval = -99
			SELECT @retval 
		END CATCH
		
END
GO

CREATE PROCEDURE usp_AddNewProduct
(
	@ProductId CHAR(4),
	@ProductName VARCHAR(50),
	@CategoryId TINYINT,
	@Price NUMERIC(8),
	@QuantityAvailable INT
)
AS
BEGIN
	DECLARE @retval int
	BEGIN TRY
		IF (@ProductId IS NULL)
			SET @retval = -1
		ELSE IF (@ProductId NOT LIKE 'P%' or LEN(@ProductId)<>4)
			SET @retval = -2
		ELSE IF (@ProductName IS NULL)
			SET @retval = -3
		ELSE IF (@CategoryId IS NULL)
			SET @retval = -4
		ELSE IF NOT EXISTS(SELECT CategoryId FROM Categories WHERE CategoryId=@CategoryId)
			SET @retval = -5
		ELSE IF (@Price<=0 OR @Price IS NULL)
			SET @retval = -6
		ELSE IF (@QuantityAvailable<0 OR @QuantityAvailable IS NULL)
			SET @retval = -7
		ELSE
			BEGIN
				INSERT INTO Products VALUES 
				(@ProductId,@ProductName, @CategoryId, @Price, @QuantityAvailable)
				SET @retval = 1
			END
		SELECT @retval 
	END TRY
	BEGIN CATCH
		SET @retval = -99
		SELECT @retval 
	END CATCH
END


--Insertion into tables
SET IDENTITY_INSERT Roles ON
INSERT INTO Roles (RoleId, RoleName) VALUES (1, 'Admin')
INSERT INTO Roles (RoleId, RoleName) VALUES (2, 'Customer')
SET IDENTITY_INSERT Roles OFF
GO

SET IDENTITY_INSERT Categories ON
INSERT INTO Categories (CategoryId, CategoryName) VALUES (1, 'Motors')
INSERT INTO Categories (CategoryId, CategoryName) VALUES (2, 'Fashion')
INSERT INTO Categories (CategoryId, CategoryName) VALUES (3, 'Electronics')
INSERT INTO Categories (CategoryId, CategoryName) VALUES (4, 'Arts')
INSERT INTO Categories (CategoryId, CategoryName) VALUES (5, 'Home')
INSERT INTO Categories (CategoryId, CategoryName) VALUES (6, 'Sporting Goods')
INSERT INTO Categories (CategoryId, CategoryName) VALUES (7, 'Toys')
SET IDENTITY_INSERT Categories OFF
GO

-- insertion script for Productss
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P101','Lamborghini Gallardo Spyder',1,18000000.00,10)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P102','BMW X1',1,3390000.00,10)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P103','BMW Z4',1,6890000.00,10)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P104','Harley Davidson Iron 883 ',1,700000.00,10)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P105','Ducati Multistrada',1,2256000.00,10)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P106','Honda CBR 250R',1,193000.00,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P107','Kenneth Cole Black & White Leather Reversible Belt',2,2500.00,50)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P108','Classic Brooks Brothers 346 Wool Black Sport Coat',2,3078.63,10)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P109','Ben Sherman Mens Necktie Silk Tie',2,1847.18,20)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P110','BRIONI Shirt Cotton NWT Medium',2,2050.00,25)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P111','Patagonia NWT mens XL Nine Trails Vest',2,2299.99,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P112','Blue Aster Blue Ivory Rugby Pack Shoes',2,6772.37,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P113','Ann Taylor 100% Cashmere Turtleneck Sweater',2,3045.44,80)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P114','Fashion New Slim Ladies Womens Suit Coat',2,2159.59,65)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P115','Apple IPhone 5s 16GB',3,52750.00,70)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P116','Samsung Galaxy S4',3,38799.99,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P117','Nokia Lumia 1320',3,42199.00,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P118','LG Nexus 5',3,32649.54,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P119','Moto DroidX',3,32156.45,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P120','Apple MAcbook Pro',3,56800.00,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P121','Dell Inspiron',3,36789.00,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P122','IPad Air',3,28000.00,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P123','Xbox 360 with kinect',3,25000.00,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P124','Abstract Hand painted Oil Painting on Canvas',4,2056.00,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P125','Mysore Painting of Lord Shiva',4,5000.00,10)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P126','Tanjore Painting of Ganesha',4,8000.00,20)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P127','Marble Elephants statue',4,9056.00,50)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P128','Wooden photo frame',4,150.00,200)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P129','Gold plated dancing peacock',4,350.00,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P130','Kundan jewellery set',4,2000.00,30)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P131','Marble chess board','4','3000.00','20')
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P132','German Folk Art Wood Carvings Shy Boy and Girl',4,6122.20,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P133','Modern Abstract Metal Art Wall Sculpture',5,5494.55,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P134','Bean Bag Chair Love Seat',5,5754.55,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P135','Scented rose candles',5,200.00,50)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P136','Digital bell chime',5,800.00,10)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P137','Curtains',5,600.00,20)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P138','Wall stickers',5,200.00,30)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P139','Shades of Blue Line-by-Line Quilt',5,691.24,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P140','Tahoe Gear Prescott 10 Person Family Cabin Tent',6,9844.33,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P141','Turner Sultan 29er Large',6,147612.60,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P142','BAMBOO BACKED HICKORY LONGBOW ',6,5291.66,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P143','Adidas Shoes',6,700.00,150)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P144','Tennis racket',6,200.00,150)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P145','Baseball glove',6,150.00,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P146','Door gym',6,700.00,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P147','Cricket bowling machine',6,3000.00,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P148','ROLLER DERBY SKATES',6,3079.99,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P149','Metal 3.5-Channel RC Helicopter',7,2458.20,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P150','Ned Butterfly Style Yo Yo',7,553.23,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P151','Baby Einstein Hand Puppets',7,1229.41,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P152','fire godzilla toy',7,614.09,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P153','Remote car',7,1000.00,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P154','Barbie doll set',7,500.00,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P155','Teddy bear',7,300.00,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P156','Clever sticks',7,400.00,100)
INSERT INTO Products(ProductId,ProductName,CategoryId,Price,QuantityAvailable) VALUES('P157','See and Say',7,200.00,50)
GO


----DELETE ALL FUNCTIONS
--Declare @sql NVARCHAR(MAX) = N'';
--SELECT @sql = @sql + N' DROP FUNCTION ' 
--                   + QUOTENAME(SCHEMA_NAME(schema_id)) 
--                   + N'.' + QUOTENAME(name)
--FROM sys.objects
--WHERE type_desc LIKE '%FUNCTION%';

--Exec sp_executesql @sql
--GO

----DROP STORED PROCEDURE
--Declare @sql NVARCHAR(MAX) = N'';
--SELECT @sql = @sql + N' DROP PROCEDURE ' 
--                   + QUOTENAME(SCHEMA_NAME(schema_id)) 
--                   + N'.' + QUOTENAME(name)
--FROM sys.objects
--WHERE type_desc LIKE '%PROCEDURE%';

--Exec sp_executesql @sql
--GO


