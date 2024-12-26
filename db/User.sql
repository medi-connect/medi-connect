-- Create table
CREATE TABLE dbo.UserAccount
(
    user_id       INT PRIMARY KEY IDENTITY (1000,1),
    email         NVARCHAR (255) NOT NULL UNIQUE,
    password      NVARCHAR (255) NOT NULL,
    status        TINYINT NOT NULL,
    sys_timestamp DATETIME DEFAULT GETDATE(),
    sys_created   DATETIME DEFAULT GETDATE()
);

-- TRIGGERS
CREATE TRIGGER trUserAccountUpdate
    ON dbo.UserAccount
    AFTER UPDATE
    AS
BEGIN
    -- Set the sys_timestamp field to the current date and time for the updated records
    UPDATE dbo.UserAccount
    SET sys_timestamp = GETDATE()
    FROM dbo.UserAccount e
             INNER JOIN inserted i ON e.user_id = i.user_id;
END;

