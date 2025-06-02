CREATE TABLE AppUser
(
    UserId      INT IDENTITY(1,1) PRIMARY KEY,
    UserName    NVARCHAR(100) NOT NULL,
    Password    NVARCHAR(MAX) NOT NULL,
    RecordDate  DATETIME2 DEFAULT(SYSDATETIME()) NOT NULL
);

CREATE UNIQUE INDEX UX_AppUser_UserName ON AppUser(UserName);

CREATE TABLE Customer
(
    CustomerId INT IDENTITY(1,1) PRIMARY KEY,
    TaxNumber  NVARCHAR(50)  NOT NULL,
    Title      NVARCHAR(200) NOT NULL,
    Address    NVARCHAR(500),
    Email      NVARCHAR(320),
    RecordDate DATETIME2 DEFAULT(SYSDATETIME()) NOT NULL,
    UserId     INT NOT NULL REFERENCES AppUser(UserId),
);

CREATE UNIQUE INDEX UX_Customer_TaxNumber ON Customer(TaxNumber);

CREATE TABLE Invoice
(
    InvoiceId     INT IDENTITY(1,1) PRIMARY KEY,
    InvoiceNumber NVARCHAR(50) NOT NULL UNIQUE,
    InvoiceDate   DATE NOT NULL,
    TotalAmount   DECIMAL(18,2) NOT NULL,
    RecordDate    DATETIME2 DEFAULT(SYSDATETIME()) NOT NULL,
    CustomerId    INT NOT NULL REFERENCES Customer(CustomerId) ON DELETE CASCADE,
    UserId        INT NOT NULL REFERENCES AppUser(UserId)
);

CREATE TABLE InvoiceLine
(
    InvoiceLineId INT IDENTITY(1,1) PRIMARY KEY,
    ItemName      NVARCHAR(200) NOT NULL,
    Quantity      INT NOT NULL,
    Price         DECIMAL(18,2) NOT NULL,
    RecordDate    DATETIME2 DEFAULT(SYSDATETIME()) NOT NULL,
    InvoiceId     INT NOT NULL REFERENCES Invoice(InvoiceId) ON DELETE CASCADE,
    UserId        INT NOT NULL REFERENCES AppUser(UserId),
);

CREATE INDEX IX_InvoiceLine_InvoiceId ON InvoiceLine(InvoiceId);
CREATE INDEX IX_InvoiceLine_UserId    ON InvoiceLine(UserId);