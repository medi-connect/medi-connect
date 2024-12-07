-- Create table
CREATE TABLE dbo.UserAccount
(
    id            INT PRIMARY KEY IDENTITY (1000,1),
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
             INNER JOIN inserted i ON e.ID = i.ID;
END;

-- ============================
--         PROCEDURES
CREATE PROCEDURE dbo.InsertUserAccount
    @Email NVARCHAR(255),
    @Password NVARCHAR(255),
    @Status TINYINT,
    @Id INT OUTPUT
AS
BEGIN
    INSERT INTO dbo.UserAccount (email, password, status)
    VALUES (@Email, @Password, @Status);
    SET @Id = SCOPE_IDENTITY();
END;

CREATE PROCEDURE dbo.GetUserCredentials
    @Email    NVARCHAR(255),
    @UserId   NVARCHAR(255) OUTPUT,
    @Password NVARCHAR(255) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT @UserId = id, @Password = password FROM DBO.UserAccount WHERE email = @email;
    IF @UserId IS NULL
    BEGIN
        -- Does not exist
        SET @Password = NULL;
    END
END;