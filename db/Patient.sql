-- Create table
CREATE TABLE dbo.Patient
(
    user_id       INT PRIMARY KEY ,
    name          NVARCHAR (255) NOT NULL,
    surname       NVARCHAR (255) NOT NULL,
    birth_date    DATE NOT NULL,
    sys_timestamp DATETIME DEFAULT GETDATE(),
    sys_created   DATETIME DEFAULT GETDATE()
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
             INNER JOIN inserted i ON e.user_id = i.user_id;
END;