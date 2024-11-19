-- Create table
CREATE TABLE dbo.UserAccount
(
    id            INT PRIMARY KEY IDENTITY (1000,1),
    email         NVARCHAR (255) NOT NULL,
    password      NVARCHAR (255) NOT NULL,
    status        TINYINT NOT NULL,
    sys_timestamp DATETIME,
    sys_created   DATETIME
);

-- Create INSERT trigger
CREATE TRIGGER trUserAccountInsert
    ON dbo.UserAccount
    INSTEAD OF INSERT
    AS
BEGIN

    INSERT INTO dbo.UserAccount (email, password, status, sys_timestamp, sys_created)
    SELECT email, password,status, GETDATE(), GETDATE()
    FROM inserted;
END;

-- Create UPDATE trigger
CREATE TRIGGER trUserAccountUpdate
    ON dbo.UserAccount
    AFTER UPDATE
    AS
BEGIN
    -- Set the sys_timestamp field to the current date and time for the updated records
    UPDATE dbo.UserAccount
    SET sys_timestamp = GETDATE()
    FROM dbo.UserAccount e
             INNER JOIN inserted i ON e.ID = i.ID;
END;




