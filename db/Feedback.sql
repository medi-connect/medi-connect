-- Create table
CREATE TABLE dbo.Feedback
(
    feedback_id    INT PRIMARY KEY IDENTITY (1000,1),
    rate           TINYINT NOT NULL,
    review         NVARCHAR (4000) NOT NULL,
    appointment_id INT NOT NULL,
    sys_timestamp  DATETIME DEFAULT GETDATE(),
    sys_created    DATETIME DEFAULT GETDATE()
);

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
             INNER JOIN inserted i ON e.feedback_id = i.feedback_id;
END;