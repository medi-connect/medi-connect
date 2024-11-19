-- Create table
CREATE TABLE dbo.Feedback
(
    id            INT PRIMARY KEY IDENTITY (1,1),
    rate          TINYINT NOT NULL,
    review        NVARCHAR (4000) NOT NULL,
    sys_timestamp DATETIME,
    sys_created   DATETIME
);

-- Create INSERT trigger
CREATE TRIGGER trFeedbackInsert
    ON dbo.Feedback
    INSTEAD OF INSERT
    AS
BEGIN

    INSERT INTO dbo.Feedback (rate, review, sys_timestamp, sys_created)
    SELECT rate, review, GETDATE(), GETDATE()
    FROM inserted;
END;

-- Create UPDATE trigger
CREATE TRIGGER trFeedbackUpdate
    ON dbo.Feedback
    AFTER UPDATE
    AS
BEGIN
    -- Set the sys_timestamp field to the current date and time for the updated records
    UPDATE dbo.Feedback
    SET sys_timestamp = GETDATE()
    FROM dbo.Feedback e
             INNER JOIN inserted i ON e.ID = i.ID;
END;