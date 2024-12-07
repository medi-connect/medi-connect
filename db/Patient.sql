-- Create table
CREATE TABLE dbo.Patient
(
    id            INT PRIMARY KEY IDENTITY (1000,1),
    name          NVARCHAR (255) NOT NULL,
    surname       NVARCHAR (255) NOT NULL,
    birth_date    DATE NOT NULL,
    user_id       INT NOT NULL,
    sys_timestamp DATETIME,
    sys_created   DATETIME
);

-- Create UPDATE trigger
CREATE TRIGGER trPatientUpdate
    ON dbo.Patient
    AFTER UPDATE
    AS
BEGIN
    -- Set the sys_timestamp field to the current date and time for the updated records
    UPDATE dbo.Patient
    SET sys_timestamp = GETDATE()
    FROM dbo.Patient e
             INNER JOIN inserted i ON e.ID = i.ID;
END;


-- ============================
--         PROCEDURES
CREATE PROCEDURE dbo.InsertPatientAccount
    @UserId     INT,
    @Name       NVARCHAR(255),
    @Surname    NVARCHAR(255),
    @BirthDate  DATE,
    @Id         INT OUTPUT
AS
BEGIN
    INSERT INTO dbo.Patient (name, surname, birth_date, user_id, sys_timestamp, sys_created)
    VALUES (@Name, @Surname, @BirthDate, @UserId, GETDATE(), GETDATE());

    SET @Id = SCOPE_IDENTITY();
END;