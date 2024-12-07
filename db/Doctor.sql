-- Create table
CREATE TABLE dbo.Doctor
(
    id            INT PRIMARY KEY IDENTITY (1000, 1),
    name          NVARCHAR (255) NOT NULL,
    surname       NVARCHAR (255) NOT NULL,
    speciality    NVARCHAR (255),
    user_id       INT,
    sys_timestamp DATETIME,
    sys_created   DATETIME
);

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
CREATE PROCEDURE InsertDoctorAccount
    @UserId INT,
    @Name   NVARCHAR(50),
    @Surname NVARCHAR(50),
    @Speciality NVARCHAR(100),
    @Id         INT OUTPUT
AS
BEGIN
    -- Ensure you handle errors/exceptions
BEGIN TRY
        -- Insert data into Employees table
INSERT INTO dbo.Doctor (name, surname, speciality, user_id, sys_timestamp, sys_created)
VALUES (@Name,@Surname, @Speciality, @UserId, GETDATE(), GETDATE());

SET @Id = SCOPE_IDENTITY();


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
