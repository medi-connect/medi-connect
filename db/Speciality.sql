-- Create table
CREATE TABLE dbo.Speciality
(
    id            INT PRIMARY KEY,
    title         NVARCHAR (255) NOT NULL,
    description   NVARCHAR (255) NOT NULL,
    sys_timestamp DATETIME,
    sys_created   DATETIME
);

-- Create INSERT trigger
CREATE TRIGGER trSpecialityInsert
    ON dbo.Speciality
    INSTEAD OF INSERT
    AS
BEGIN

    INSERT INTO dbo.Speciality (title, description, sys_timestamp, sys_created)
    SELECT title, description, GETDATE(), GETDATE()
    FROM inserted;
END;

-- Create UPDATE trigger
CREATE TRIGGER trSpecialityUpdate
    ON dbo.Speciality
    AFTER UPDATE
    AS
BEGIN
    -- Set the sys_timestamp field to the current date and time for the updated records
    UPDATE dbo.Speciality
    SET sys_timestamp = GETDATE()
    FROM dbo.Speciality e
             INNER JOIN inserted i ON e.ID = i.ID;
END;



