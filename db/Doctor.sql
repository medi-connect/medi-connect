-- Create table
CREATE TABLE dbo.Doctor
(
    id            INT PRIMARY KEY,
    name          NVARCHAR (255) NOT NULL,
    surname       NVARCHAR (255) NOT NULL,
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

