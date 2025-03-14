using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreditCardRegistration.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if the CardTypes table exists; if not, create it
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CardTypes')
                BEGIN
                    CREATE TABLE CardTypes (
                        CardTypeID INT NOT NULL IDENTITY(1,1),
                        CardTypeName NVARCHAR(50) NOT NULL,
                        Benefits NVARCHAR(200) NOT NULL,
                        CONSTRAINT PK_CardTypes PRIMARY KEY (CardTypeID)
                    );
                END
            ");

            // Check if the Users table exists; if not, create it
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
                BEGIN
                    CREATE TABLE Users (
                        UserID INT NOT NULL IDENTITY(1,1),
                        FullName NVARCHAR(100) NOT NULL,
                        Email NVARCHAR(100) NOT NULL,
                        Phone NVARCHAR(20) NOT NULL,
                        DateOfBirth DATETIME2 NOT NULL,
                        IDNumber NVARCHAR(20) NOT NULL,
                        MonthlyIncome DECIMAL(18,2) NOT NULL,
                        Occupation NVARCHAR(50) NOT NULL,
                        Address NVARCHAR(200) NOT NULL,
                        Username NVARCHAR(50) NOT NULL,
                        PasswordHash NVARCHAR(200) NOT NULL,
                        CONSTRAINT PK_Users PRIMARY KEY (UserID)
                    );
                END
            ");

            // Check if the CreditCards table exists; if not, create it
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CreditCards')
                BEGIN
                    CREATE TABLE CreditCards (
                        CreditCardID INT NOT NULL IDENTITY(1,1),
                        UserID INT NOT NULL,
                        CardTypeID INT NOT NULL,
                        CardNumber NVARCHAR(19) NOT NULL,
                        ExpiryDate NVARCHAR(5) NOT NULL,
                        CVV NVARCHAR(3) NOT NULL,
                        FrontID NVARCHAR(200) NOT NULL,
                        BackID NVARCHAR(200) NOT NULL,
                        DocumentPath NVARCHAR(200) NOT NULL,
                        Status NVARCHAR(20) NOT NULL,
                        CreatedDate DATETIME2 NOT NULL,
                        CONSTRAINT PK_CreditCards PRIMARY KEY (CreditCardID),
                        CONSTRAINT FK_CreditCards_CardTypes_CardTypeID FOREIGN KEY (CardTypeID) 
                            REFERENCES CardTypes(CardTypeID) ON DELETE CASCADE,
                        CONSTRAINT FK_CreditCards_Users_UserID FOREIGN KEY (UserID) 
                            REFERENCES Users(UserID) ON DELETE CASCADE
                    );
                END
            ");

            // Check and create indexes if they do not exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CreditCards_CardTypeID' AND object_id = OBJECT_ID('CreditCards'))
                BEGIN
                    CREATE INDEX IX_CreditCards_CardTypeID ON CreditCards(CardTypeID);
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CreditCards_UserID' AND object_id = OBJECT_ID('CreditCards'))
                BEGIN
                    CREATE INDEX IX_CreditCards_UserID ON CreditCards(UserID);
                END
            ");

            // Seed data for CardTypes, only insert if no data exists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM CardTypes WHERE CardTypeName IN ('Visa', 'Mastercard', 'Amex'))
                BEGIN
                    INSERT INTO CardTypes (CardTypeName, Benefits)
                    VALUES
                        ('Visa', '5% cashback on shopping'),
                        ('Mastercard', 'Free airport lounge access'),
                        ('Amex', 'Exclusive travel benefits');
                END
            ");

            // Log success message after all operations
            migrationBuilder.Sql(@"
                PRINT 'Database update completed successfully. The schema and seed data have been applied.';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Check and drop the table if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.tables WHERE name = 'CreditCards')
                BEGIN
                    DROP TABLE CreditCards;
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.tables WHERE name = 'CardTypes')
                BEGIN
                    DROP TABLE CardTypes;
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
                BEGIN
                    DROP TABLE Users;
                END
            ");
        }
    }
}