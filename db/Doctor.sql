-- Create table
CREATE TABLE dbo.Doctor
(
    id            INT PRIMARY KEY IDENTITY (1000, 1),
    name          NVARCHAR2 (255) NOT NULL,
    surname       NVARCHAR2 (255) NOT NULL,
    speciality    NVARCHAR2 (255),
    sex           TINYINT,
    sys_timestamp DATETIME,
    sys_created   DATETIME
);

-- Create INSERT trigger
CREATE TRIGGER trDoctorInsert
    ON dbo.Doctor
    INSTEAD OF INSERT
    AS
BEGIN

    INSERT INTO dbo.Doctor (name, surname, sys_timestamp, sys_created)
    SELECT name, surname, GETDATE(), GETDATE()
    FROM inserted;
END;

-- Create UPDATE trigger
CREATE TRIGGER trDoctorUpdate
    ON dbo.Doctor
    AFTER UPDATE
    AS
BEGIN
    -- Set the sys_timestamp field to the current date and time for the updated records
    UPDATE dbo.Doctor
    SET sys_timestamp = GETDATE()
    FROM dbo.Doctor e
             INNER JOIN inserted i ON e.ID = i.ID;
END;


-- PROCEDURES --

CREATE PROCEDURE InsertDoctor
    @Name NVARCHAR(50),
    @Surname NVARCHAR(50),
    @Email NVARCHAR(100)
AS
BEGIN
    -- Ensure you handle errors/exceptions
BEGIN TRY
        -- Insert data into Employees table
INSERT INTO dbo.Doctor (name, surname, speciality)
        VALUES (@@, @FirstName, @LastName, @Email);

END TRY
BEGIN CATCH
        -- Error handling
        DECLARE @ErrorMessage NVARCHAR(4000);
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;

SELECT
    @ErrorMessage = ERROR_MESSAGE(),
    @ErrorSeverity = ERROR_SEVERITY(),
    @ErrorState = ERROR_STATE();

-- Raise the error back to the caller
RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH
END;
