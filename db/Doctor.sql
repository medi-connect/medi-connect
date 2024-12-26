-- Create table
CREATE TABLE dbo.Doctor
(
    user_id       INT PRIMARY KEY,
    name          NVARCHAR (255) NOT NULL,
    surname       NVARCHAR (255) NOT NULL,
    speciality    NVARCHAR (255),
    sys_timestamp DATETIME DEFAULT GETDATE(),
    sys_created   DATETIME DEFAULT GETDATE()
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
             INNER JOIN inserted i ON e.user_id = i.user_id;
END;
