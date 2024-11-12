CREATE DATABASE FUHRM;
GO

USE FUHRM;
GO

CREATE TABLE Roles (
    RoleID INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(100) NOT NULL
);

CREATE TABLE Accounts (
    AccountID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(255) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    RoleID INT NOT NULL,
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);

CREATE TABLE Departments (
    DepartmentID INT PRIMARY KEY IDENTITY(1,1),
    DepartmentName NVARCHAR(100) NOT NULL,
    CreateDate DATETIME DEFAULT GETDATE(),
);

CREATE TABLE Positions (
    PositionID INT PRIMARY KEY IDENTITY(1,1),
    PositionName NVARCHAR(100) NOT NULL
);

CREATE TABLE Employees (
    EmployeeID INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(255) NOT NULL,
    DateOfBirth DATE NOT NULL,
    Gender NVARCHAR(50) NOT NULL,
    Address NVARCHAR(255) NOT NULL,
    PhoneNumber NVARCHAR(20) NOT NULL,
    DepartmentID INT NOT NULL,
    PositionID INT NOT NULL,
    AccountID INT NOT NULL,
    Salary FLOAT NOT NULL,
    StartDate DATE NOT NULL,
    ProfilePicture NVARCHAR(255),
    FOREIGN KEY (DepartmentID) REFERENCES Departments(DepartmentID),
    FOREIGN KEY (PositionID) REFERENCES Positions(PositionID),
    FOREIGN KEY (AccountID) REFERENCES Accounts(AccountID)
);

CREATE TABLE Salaries (
    SalaryID INT PRIMARY KEY IDENTITY(1,1),
    EmployeeID INT NOT NULL,
    BaseSalary FLOAT NOT NULL,
    Allowance FLOAT,
    Bonus FLOAT,
    Penalty FLOAT,
    TotalIncome AS (BaseSalary + Allowance + Bonus - Penalty) PERSISTED,
    PaymentDate DATE NOT NULL,
    FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID)
);

CREATE TABLE Attendances (
    AttendanceID INT PRIMARY KEY IDENTITY(1,1),
    EmployeeID INT NOT NULL,
    Date DATE NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    OvertimeHours INT,
    FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID)
);

CREATE TABLE Notifications (
    NotificationID INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(255) NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    DepartmentID INT NOT NULL,
    CreatedDate DATETIME NOT NULL,
    FOREIGN KEY (DepartmentID) REFERENCES Departments(DepartmentID)
);

CREATE TABLE LeaveRequests (
    LeaveRequestID INT PRIMARY KEY IDENTITY(1,1),
    EmployeeID INT NOT NULL,
    LeaveType NVARCHAR(100) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID)
);


-- Chèn dữ liệu vào bảng Roles
INSERT INTO Roles (RoleName) VALUES ('Admin');
INSERT INTO Roles (RoleName) VALUES ('Employee');

-- Chèn dữ liệu vào bảng Accounts
INSERT INTO Accounts (Username, Password, RoleID) VALUES ('admin', 'admin123', 1);
INSERT INTO Accounts (Username, Password, RoleID) VALUES ('ts1', 'ts1', 2);
INSERT INTO Accounts (Username, Password, RoleID) VALUES ('kt1', 'kt1', 2);
INSERT INTO Accounts (Username, Password, RoleID) VALUES ('it1', 'it1', 2);

-- Chèn dữ liệu vào bảng Departments
INSERT INTO Departments (DepartmentName, CreateDate) VALUES (N'Tuyển sinh', GETDATE());
INSERT INTO Departments (DepartmentName, CreateDate) VALUES (N'Kế toán', GETDATE());
INSERT INTO Departments (DepartmentName, CreateDate) VALUES ('IT', GETDATE());
INSERT INTO Departments (DepartmentName, CreateDate) VALUES (N'Giảng viên', GETDATE());

-- Chèn dữ liệu vào bảng Positions
INSERT INTO Positions (PositionName) VALUES (N'Trưởng phòng');
INSERT INTO Positions (PositionName) VALUES (N'Phó phòng');
INSERT INTO Positions (PositionName) VALUES (N'Nhân viên');
INSERT INTO Positions (PositionName) VALUES (N'Trưởng bộ môn');

-- Chèn dữ liệu vào bảng Employees
INSERT INTO Employees (FullName, DateOfBirth, Gender, Address, PhoneNumber, DepartmentID, PositionID, AccountID, Salary, StartDate, ProfilePicture) 
VALUES (N'Minh Huệ', '1985-05-15', N'Nữ', N'Phú Thọ', '1234567890', 1, 1, 2, 5000000, '2020-01-01', 'hue.jpg');

INSERT INTO Employees (FullName, DateOfBirth, Gender, Address, PhoneNumber, DepartmentID, PositionID, AccountID, Salary, StartDate, ProfilePicture) 
VALUES (N'Đức Vũ', '1990-07-20', N'Nam', N'Quảng Ninh', '9876543210', 2, 2, 3, 6000000, '2021-02-01', 'vu.jpg');

INSERT INTO Employees (FullName, DateOfBirth, Gender, Address, PhoneNumber, DepartmentID, PositionID, AccountID, Salary, StartDate, ProfilePicture) 
VALUES (N'Việt An', '1992-03-10', N'Nam', N'Nghệ An', '5556667777', 3, 3, 4, 5500000, '2022-03-01', 'an.jpg');

-- Chèn dữ liệu vào bảng Salaries
INSERT INTO Salaries (EmployeeID, BaseSalary, Allowance, Bonus, Penalty, PaymentDate) 
VALUES (1, 500000, 5000, 2000, 500, '2024-11-11');

INSERT INTO Salaries (EmployeeID, BaseSalary, Allowance, Bonus, Penalty, PaymentDate) 
VALUES (2, 600000, 6000, 2500, 600, '2024-11-11');

INSERT INTO Salaries (EmployeeID, BaseSalary, Allowance, Bonus, Penalty, PaymentDate) 
VALUES (3, 550000, 5500, 2200, 550, '2024-11-11');


-- Chèn dữ liệu vào bảng Attendances
INSERT INTO Attendances (EmployeeID, Date, Status, OvertimeHours) 
VALUES (1, '2024-10-11', 'Present', 2);

INSERT INTO Attendances (EmployeeID, Date, Status, OvertimeHours) 
VALUES (2, '2024-10-11', 'Present', 1);

INSERT INTO Attendances (EmployeeID, Date, Status, OvertimeHours) 
VALUES (3, '2024-09-11', 'Present', 0);

-- Chèn dữ liệu vào bảng Notifications
INSERT INTO Notifications (Title, Content, DepartmentID, CreatedDate) 
VALUES (N'Họp', N'Cuộc họp bắt đầu lúc 10:00.', 1, '2024-11-11 09:00:00');

INSERT INTO Notifications (Title, Content, DepartmentID, CreatedDate) 
VALUES (N'Cập nhật quy định', N'Đừng quên cập nhật các quy định mới.', 2, '2024-11-12 10:00:00');

INSERT INTO Notifications (Title, Content, DepartmentID, CreatedDate) 
VALUES (N'Bảo trì hệ thống', N'Hệ thống sẽ tiến hành bảo trì vào cuối tuần này.', 3, '2024-11-13 11:00:00');

select * from Accounts