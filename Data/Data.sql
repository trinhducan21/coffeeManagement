CREATE DATABASE QuanLyQuanCafe
GO

USE QuanLyQuanCafe
GO

CREATE TABLE TableFood
(
	id INT IDENTITY PRIMARY KEY, 
	name NVARCHAR(100) DEFAULT N'Unnamed table',
	status NVARCHAR(100) DEFAULT N'Empty'
)
GO

CREATE TABLE Account
(
	DisplayName NVARCHAR(100) NOT NULL DEFAULT N'CoffeeShopStaff',
	UserName NVARCHAR(100) PRIMARY KEY,
	PassWord NVARCHAR(1000) NOT NULL DEFAULT N'18833213210117723916811824913021616923162239', 
	Type INT NOT NULL
)
GO

CREATE TABLE FoodCategory
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR (100) DEFAULT N'Unnamed'
)
GO


CREATE TABLE Food
(
	id INT IDENTITY PRIMARY KEY, 
	name NVARCHAR(100) UNIQUE NOT NULL DEFAULT N'Unnamed food',
	idCategory INT NOT NULL,
	price FLOAT NOT NULL DEFAULT 0
)
GO

CREATE TABLE Bill
(
	id INT IDENTITY PRIMARY KEY,
	DateCheckIn DATETIME2 NOT NULL DEFAULT GETDATE(),
	DateCheckOut DATETIME2,
	idTable INT NOT NULL,
	discount INT DEFAULT 0,
	status INT NOT NULL,
	totalPrice FLOAT DEFAULT 0
)
GO

CREATE TABLE BillInfo
(
	id INT IDENTITY PRIMARY KEY,
	idBill INT NOT NULL,
	idFood INT NOT NULL,
	count INT NOT NULL DEFAULT 0
)
GO

-- Thêm Account
INSERT INTO dbo.Account(UserName, DisplayName, PassWord, Type)
VALUES 
(N'staff1', N'CoffeeShopStaff', N'18833213210117723916811824913021616923162239', 0),
(N'UKenshin', N'Uesugi Kenshin', N'23618210643915568392142525056116228251255', 1), 
(N'TShingen', N'Takeda Shingen', N'922131781105498207216134253149331979230', 1),
(N'staff', N'CoffeeShopStaff', N'18833213210117723916811824913021616923162239', 0)
GO

-- Thêm Table
DECLARE @i INT = 1
WHILE @i <= 35
BEGIN
	INSERT INTO dbo.TableFood(name, status)
	VALUES (N'Table ' + CAST(@i AS NVARCHAR(10)), N'Empty')
	SET @i = @i + 1
END
GO

UPDATE dbo.TableFood SET status = N'Occupied' WHERE id = 1 OR id = 8
GO

-- Thêm Category
INSERT INTO dbo.FoodCategory(name)
VALUES 
(N'Tea'), 
(N'Coffee'), 
(N'Juice'), 
(N'Bread'), 
(N'Cake'), 
(N'Combo')
GO

-- Thêm Food
INSERT INTO dbo.Food(name, idCategory, price)
VALUES
(N'Golden Lotus Tea', 1, 45000),
(N'Lychee Jelly Tea', 1, 45000),
(N'Peach Jelly Tea', 1, 45000),
(N'Iced Black Filter Coffee', 2, 29000),
(N'White Coffee', 2, 29000),
(N'Americano Coffee', 2, 30000),
(N'Orange Juice', 3, 50000),
(N'Iced Lemonade', 3, 39000),
(N'Apple Juice', 3, 35000),
(N'Pate Breadsticks', 4, 10000),
(N'Chicken Cheese Breadsticks', 4, 19000),
(N'Beef Cheese Breadsticks', 4, 25000),
(N'Tiramisu', 5, 35000),
(N'Croissant', 5, 29000),
(N'Banana Cake', 5, 29000),
(N'Combo 1 Afternoon Tea', 6, 79000),
(N'Combo 2 Breakfast', 6, 59000)
GO

-- Thêm Bill
INSERT INTO dbo.Bill(DateCheckIn, DateCheckOut, idTable, status)
VALUES 
('2024-07-19T14:30:00', '2024-07-19T19:30:00', 1, 1),
('2024-07-19T14:32:25', '2024-07-19T17:13:21', 2, 1),
(GETDATE(), NULL, 1, 0),
(GETDATE(), NULL, 8, 0)
GO

-- Thêm BillInfo
INSERT INTO dbo.BillInfo(idBill, idFood, count)
VALUES
(1, 1, 2),
(1, 2, 1),
(1, 3, 1),
(2, 4, 1),
(2, 5, 2),
(2, 6, 1),
(3, 7, 3),
(3, 8, 1),
(3, 9, 1),
(4, 17, 1)
GO

-- Procedure
CREATE PROC USP_GetAccountByUserName
	@userName NVARCHAR(100)
AS
BEGIN
	SELECT * FROM dbo.Account WHERE UserName = @userName
END
GO


CREATE PROC USP_Login
	@userName NVARCHAR(100),
	@passWord NVARCHAR(100)
AS 
BEGIN
    SELECT * FROM dbo.Account
    WHERE UserName = @UserName AND PassWord = @PassWord
END
GO


CREATE PROC USP_GetTableList
AS
BEGIN
	SELECT * FROM dbo.TableFood
END
GO


CREATE PROC USP_InsertBill
	@idTable INT
AS
BEGIN
	INSERT INTO dbo.Bill(DateCheckIn, idTable, status)
	VALUES (GETDATE(), @idTable, 0)
END
GO


CREATE PROC USP_InsertBillInfo
	@idBill INT,
	@idFood INT,
	@count INT
AS
BEGIN
	DECLARE @isExistBillInfo INT
	DECLARE @foodCount INT = 1

	SELECT @isExistBillInfo = id, @foodCount = b.count
	FROM dbo.BillInfo b
	WHERE b.idBill = @idBill AND b.idFood = @idFood

	IF (@isExistBillInfo > 0)
	BEGIN
		DECLARE @newCount INT = @foodCount + @count
		IF (@newCount > 0)
			UPDATE dbo.BillInfo SET count = @foodCount + @count WHERE idFood = @idFood 
		ELSE
			DELETE dbo.BillInfo WHERE idBill = @idBill AND idFood = @idFood
	END
	ELSE
	BEGIN
		INSERT INTO dbo.BillInfo(idBill, idFood, count)
		VALUES (@idBill, @idFood, @count)
	END
END
GO

CREATE PROC USP_GetListBillByDate
	@checkInDate DATETIME,
	@checkOutDate DATETIME
AS
BEGIN
	SELECT t.name AS [Table Name], b.totalPrice AS [Total Price], b.DateCheckIn AS [Check In], b.DateCheckOut AS [Check Out], b.discount AS [Discount]
	FROM dbo.Bill b, dbo.TableFood t
	WHERE b.DateCheckIn >= @checkInDate AND b.DateCheckOut <= @checkOutDate AND b.status = 1 AND t.id = b.idTable
END
GO

CREATE PROC USP_UpdateAccount
	@userName NVARCHAR(100),
	@displayName NVARCHAR(100),
	@password NVARCHAR(1000),
	@newPassword NVARCHAR(1000)
AS
BEGIN
	DECLARE @isRightPass INT = 0

	SELECT @isRightPass = COUNT(*) FROM dbo.Account WHERE UserName = @userName AND PassWord = @PassWord

	IF (@isRightPass = 1)
	BEGIN
		IF (@newPassWord = NULL OR @newPassWord = '2122914021714301784233128915223624866126')
			UPDATE dbo.Account SET DisplayName = @displayName WHERE UserName = @userName
		ELSE
			UPDATE dbo.Account SET DisplayName = @displayName, PassWord = @newPassWord WHERE UserName = @userName
	END
END
GO

CREATE PROC USP_GetListBillByDateAndPage
	@checkIn date, 
	@checkOut date, 
	@page int
AS 
BEGIN
	DECLARE @pageRows INT = 10
	DECLARE @selectRows INT = @pageRows
	DECLARE @exceptRows INT = (@page - 1) * @pageRows
	
	;WITH BillShow AS
	( 
		SELECT b.ID, t.name AS [Table Name], b.totalPrice AS [Total Price], DateCheckIn AS [Check In], DateCheckOut AS [Check out], discount AS [Discount]
		FROM dbo.Bill AS b,dbo.TableFood AS t
		WHERE DateCheckIn >= @checkIn AND DateCheckOut <= @checkOut AND b.status = 1 AND t.id = b.idTable
	)
	
	SELECT TOP (@selectRows) * FROM BillShow WHERE id NOT IN (SELECT TOP (@exceptRows) id FROM BillShow)
END
GO

CREATE PROC USP_GetNumBillByDate
	@checkInDate DATETIME,
	@checkOutDate DATETIME
AS
BEGIN
	SELECT COUNT(*) 
	FROM dbo.Bill AS b, dbo.TableFood AS t
	WHERE b.DateCheckIn >= @checkInDate AND b.DateCheckOut <= @checkOutDate AND b.status = 1 AND t.id = b.idTable
END
GO

CREATE PROC USP_DeleteFood
	@id INT
AS
BEGIN
	BEGIN TRY 
		DECLARE @Count INT

		SELECT @Count = COUNT(*)
		FROM dbo.BillInfo AS bi
		JOIN dbo.Bill AS b ON bi.idBill = b.id
		WHERE b.status = 0 AND bi.idFood = @id

		IF @Count > 0
		BEGIN
			PRINT 'Cannot delete food item because it is present in bills which have not been paid yet'
			RETURN
		END

		DELETE FROM dbo.Food WHERE id = @id
	END TRY
	BEGIN CATCH
		PRINT 'An error occurred in the stored procedure USP_DeleteFood'
	END CATCH
END
GO

CREATE PROC USP_UpdateFood
	@id INT,
	@name NVARCHAR(100),
	@idCategory INT,
	@price FLOAT
AS
BEGIN
	BEGIN TRY
		UPDATE dbo.Food
		SET name = @name, idCategory = @idCategory, price = @price
		WHERE id = @id
	END TRY
	BEGIN CATCH
		PRINT 'An error occurred in the stored procedure USP_UpdateFood'
	END CATCH
END
GO



CREATE TRIGGER UTG_UpdateBillInfo
ON dbo.BillInfo FOR INSERT, UPDATE
AS
BEGIN
	DECLARE @idBill INT
	
	SELECT @idBill = idBill FROM inserted

	DECLARE @idTable INT

	SELECT @idTable = idTable FROM dbo.Bill WHERE id = @idBill AND status = 0
 
	UPDATE dbo.TableFood SET status = N'Occupied' WHERE id = @idTable
END
GO

CREATE TRIGGER UTG_UpdateBill
ON dbo.Bill FOR UPDATE
AS
BEGIN
	DECLARE @idBill INT
	
	SELECT @idBill = id FROM inserted

	DECLARE @idTable INT

	SELECT @idTable = idTable FROM dbo.Bill WHERE id = @idBill

	DECLARE @count INT = 0

	SELECT @count = COUNT(*) FROM dbo.Bill WHERE idTable = @idTable AND status = 0

	IF (@count = 0)
		UPDATE dbo.TableFood SET status = N'Empty' WHERE id = @idTable
END
GO

CREATE TRIGGER UTG_DeleteBillInfo
ON dbo.BillInfo FOR DELETE
AS
BEGIN
	DECLARE @idBillInfo INT
	DECLARE @idBill INT
	SELECT @idBillInfo = id, @idBill = Deleted.idBill FROM deleted

	DECLARE @idTable INT
	SELECT @idTable = idTable FROM dbo.Bill WHERE id = @idBill

	DECLARE @count INT = 0

	SELECT @count = COUNT(*) FROM dbo.BillInfo AS bi, dbo.Bill AS b WHERE b.id = bi.idBill AND b.id = @idBill AND b.status = 0

	IF (@count = 0)
		UPDATE dbo.TableFood SET status = N'Empty' WHERE id = @idTable
END
GO

CREATE TRIGGER UTG_DeleteCategory
ON dbo.FoodCategory
INSTEAD OF DELETE
AS
BEGIN
    BEGIN TRY
        DECLARE @CategoryID INT;
        DECLARE @Count INT;

        -- Get the ID of the category to be deleted
        SELECT @CategoryID = id FROM deleted;

        -- Check if there are any food items in the category
        IF EXISTS (SELECT 1 FROM dbo.Food WHERE idCategory = @CategoryID)
        BEGIN
            -- Check if any of these food items are present in bills with status 0
            SELECT @Count = COUNT(*)
            FROM dbo.BillInfo AS bi
            JOIN dbo.Bill AS b ON bi.idBill = b.id
            JOIN dbo.Food AS f ON bi.idFood = f.id
            WHERE b.status = 0 AND f.idCategory = @CategoryID;

            -- If such food items are found, print a message and return
            IF @Count > 0
            BEGIN
                PRINT 'Cannot delete category because it contains food items in bills which have not been paid yet';
                RETURN;
            END
        END

        -- If no such food items are found, allow the delete operation to proceed
		DELETE FROM dbo.Food WHERE idCategory = @CategoryID;
        DELETE FROM dbo.FoodCategory WHERE id = @CategoryID;
    END TRY
    BEGIN CATCH
        -- Handle the error here without stopping the entire transaction
        PRINT 'An error occurred in the trigger UTG_DeleteCategory.';
    END CATCH
END
GO


SELECT * FROM Account
SELECT * FROM dbo.TableFood
SELECT * FROM Food
SELECT * FROM dbo.FoodCategory
SELECT * FROM Bill
SELECT * FROM BillInfo

SELECT * FROM dbo.Food WHERE idCategory = 13

DELETE dbo.FoodCategory WHERE id = 18

DELETE Food WHERE id = 44
GO

EXEC USP_DeleteFood 44

EXEC USP_UpdateFood 6, N'Combo 5 Breakfast', 6, 69000

