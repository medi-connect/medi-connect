-- Create table
CREATE TABLE dbo.Appointment
(
    appointment_id INT PRIMARY KEY IDENTITY (1000,1),
    start_time     DATETIME NOT NULL,
    end_time       DATETIME,
    title          NVARCHAR (255) NOT NULL,
    description    NVARCHAR (1000),
    status         NVARCHAR (255) DEFAULT 'Pending',
    doctor_id      INT NOT NULL,
    patient_id     INT NOT NULL,
    created_by     BIT DEFAULT 0, -- 0 - Patient, 1 - Doctor
    sys_timestamp  DATETIME DEFAULT GETDATE(),
    sys_created    DATETIME DEFAULT GETDATE()
);


-- Create UPDATE trigger
CREATE TRIGGER trAppointmentUpdate
    ON dbo.Appointment
    AFTER UPDATE
    AS
BEGIN
    -- Set the sys_timestamp field to the current date and time for the updated records
    UPDATE dbo.Appointment
    SET sys_timestamp = GETDATE()
    FROM dbo.Appointment e
             INNER JOIN inserted i ON e.appointment_id = i.appointment_id;
END;

ALTER TABLE dbo.Appointment
ADD CONSTRAINT CK_Appointment_Status
CHECK (status IN ('Pending', 'Confirmed', 'Declined', 'Canceled'));



