-- Script de creación de tablas
-- Base de datos: SQL Server
CREATE DATABASE DB_Ventas_ETL;
GO

USE DB_Ventas_ETL;
GO

-- Tabla Categorias
CREATE TABLE Category (
    CategoryID int IDENTITY(1,1) PRIMARY KEY,
    CategoryName varchar(100) NOT NULL
);

-- Tabla Estados de la venta
CREATE TABLE Status (
    StatusID int IDENTITY(1,1) PRIMARY KEY,
    StatusName varchar(50) NOT NULL
);

-- Tabla Pais
CREATE TABLE Country (
    CountryID int IDENTITY(1,1) PRIMARY KEY,
    CountryName varchar(100) NOT NULL UNIQUE
);

-- Tabla Ciudad
CREATE TABLE City (
    CityID int IDENTITY(1,1) PRIMARY KEY,
    CityName varchar(100) NOT NULL,
    CountryID int,
    FOREIGN KEY (CountryID) REFERENCES Country(CountryID)
);

-- Tabla Clientes
CREATE TABLE Customer (
    CustomerID int PRIMARY KEY,
    FirstName varchar(100) NOT NULL,
    LastName varchar(100) NOT NULL,
    Email varchar(100),
    Phone varchar(20),
    CityID int, 
    FOREIGN KEY (CityID) REFERENCES City(CityID)
);

-- Tabla Productos
CREATE TABLE Product (
    ProductID int PRIMARY KEY,
    ProductName varchar(100) NOT NULL,
    Price decimal(10, 2) NOT NULL,
    Stock int NOT NULL,
    CategoryID int,
    FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID)
);

-- Tabla Ventas
CREATE TABLE [Order] (
    OrderID int PRIMARY KEY,
    CustomerID int,
    OrderDate datetime DEFAULT GETDATE(),
    StatusID int,
    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID),
    FOREIGN KEY (StatusID) REFERENCES Status(StatusID)
);

-- Tabla Detalles de Ventas
CREATE TABLE Order_Detail (
    OrderID int,
    ProductID int,
    Quantity int NOT NULL,
    UnitPrice decimal(10, 2),
    TotalPrice decimal(10, 2),
    PRIMARY KEY (OrderID, ProductID),
    FOREIGN KEY (OrderID) REFERENCES [Order](OrderID),
    FOREIGN KEY (ProductID) REFERENCES Product(ProductID)
);


