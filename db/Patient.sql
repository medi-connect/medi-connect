-- Create table
CREATE TABLE dbo.Patient
(
    id            INT PRIMARY KEY,
    name          NVARCHAR (255) NOT NULL,
    surname       NVARCHAR (255) NOT NULL,
    birth_date    DATETIME NOT NULL,
    sys_timestamp DATETIME,
    sys_created   DATETIME
);

-- Create INSERT trigger
CREATE TRIGGER trPatientInsert
    ON dbo.Patient
    INSTEAD OF INSERT
    AS
BEGIN

    INSERT INTO dbo.Patient (name, surname, birth_date, sys_timestamp, sys_created)
    SELECT name, surname,birth_date, GETDATE(), GETDATE()
    FROM inserted;
END;

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




