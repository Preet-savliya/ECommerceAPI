USE ECommerceDB;
GO

/* ===================== USERS ===================== */
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(256) NOT NULL,
    Address NVARCHAR(250) NOT NULL
);

INSERT INTO Users (FirstName, LastName, Email, Password, Address)
VALUES
    ('John', 'Doe', 'john.doe@example.com', 'hashedpassword1', 'Mumbai'),
    ('Jane', 'Smith', 'jane.smith@example.com', 'hashedpassword2', 'Delhi'),
    ('Michael', 'Johnson', 'michael.johnson@example.com', 'hashedpassword3', 'Bangalore'),
    ('Emily', 'Davis', 'emily.davis@example.com', 'hashedpassword4', 'Chennai'),
    ('David', 'Martinez', 'david.martinez@example.com', 'hashedpassword5', 'Kolkata'),
    ('Sarah', 'Taylor', 'sarah.taylor@example.com', 'hashedpassword6', 'Hyderabad'),
    ('Chris', 'Anderson', 'chris.anderson@example.com', 'hashedpassword7', 'Pune'),
    ('Jessica', 'Thomas', 'jessica.thomas@example.com', 'hashedpassword8', 'Ahmedabad'),
    ('Daniel', 'Jackson', 'daniel.jackson@example.com', 'hashedpassword9', 'Surat'),
    ('Laura', 'White', 'laura.white@example.com', 'hashedpassword10', 'Chandigarh');


/* ===================== CATEGORIES ===================== */
CREATE TABLE Categories (
    CategoryId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(500) NULL
);

INSERT INTO Categories (Name, Description)
VALUES
    ('Home Appliances', 'All kinds of home appliances like washing machines, refrigerators, etc.'),
    ('Furniture', 'Furniture items including sofas, beds, and tables'),
    ('Electronics', 'Electronic gadgets and devices'),
    ('Clothing', 'Apparel and clothing items');


/* ===================== PRODUCTS ===================== */
CREATE TABLE Products (
    ProductId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(500) NULL,
    Price DECIMAL(18,2) NOT NULL,
    Stock INT NOT NULL,
    CategoryId INT NOT NULL,
    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId)
);

INSERT INTO Products (Name, Description, Price, Stock, CategoryId)
VALUES
    ('Washing Machine', 'High-efficiency washing machine', 499.99, 20, 1),
    ('Refrigerator', 'Double-door refrigerator with smart features', 799.99, 15, 1),
    ('Sofa Set', 'Comfortable 3-piece sofa set', 299.99, 25, 2),
    ('Dining Table', 'Wooden dining table with 6 chairs', 499.99, 30, 2),
    ('Smart TV', '55-inch 4K smart LED TV', 699.99, 10, 3),
    ('Headphones', 'Wireless noise-cancelling headphones', 149.99, 50, 3),
    ('Smartwatch', 'Fitness tracking smartwatch', 199.99, 40, 3),
    ('Jeans', 'Denim jeans in various sizes', 49.99, 100, 4),
    ('Jacket', 'Warm winter jacket', 79.99, 60, 4),
    ('Dress', 'Summer dress for women', 39.99, 80, 4);


/* ===================== ORDERS ===================== */
CREATE TABLE Orders (
    OrderId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    ProductId INT NOT NULL,
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    TotalAmount DECIMAL(18,2) NOT NULL,
    ShippingAddress NVARCHAR(250) NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);

INSERT INTO Orders (UserId, ProductId, TotalAmount, ShippingAddress)
VALUES
    (1, 5, 799.99, '789 Oak St, Mumbai'),       -- Smart TV
    (2, 1, 499.99, '101 Pine St, Delhi'),       -- Washing Machine
    (3, 10, 899.99, '202 Birch St, Bangalore'), -- Dress
    (4, 8, 599.99, '303 Cedar St, Kolkata'),    -- Jeans
    (5, 6, 229.99, '404 Maple St, Chennai'),    -- Headphones
    (6, 5, 349.99, '505 Elm St, Hyderabad'),    -- Smart TV
    (7, 9, 699.99, '606 Pine St, Pune'),        -- Jacket
    (8, 4, 479.99, '707 Oak St, Ahmedabad'),    -- Dining Table
    (9, 3, 549.99, '808 Birch St, Surat'),      -- Sofa Set
    (10, 2, 359.99, '909 Cedar St, Chandigarh');-- Refrigerator


/* ===================== CART ITEMS ===================== */
CREATE TABLE CartItems (
    CartItemId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);

INSERT INTO CartItems (UserId, ProductId, Quantity)
VALUES
    (1, 3, 1),  -- Sofa Set
    (2, 7, 2),  -- Smartwatch
    (3, 10, 3), -- Dress
    (4, 8, 1),  -- Jeans
    (5, 6, 1),  -- Headphones
    (6, 5, 2),  -- Smart TV
    (7, 9, 1),  -- Jacket
    (8, 4, 1),  -- Dining Table
    (9, 3, 1),  -- Sofa Set
    (10, 2, 1); -- Refrigerator
