-- Doctor
ALTER TABLE dbo.Doctor
    ADD CONSTRAINT FK_Doctor_User FOREIGN KEY (user_id)
        REFERENCES dbo.UserAccount(user_id);

-- Patient
ALTER TABLE dbo.Patient
    ADD CONSTRAINT FK_Patient_User FOREIGN KEY (user_id)
        REFERENCES dbo.UserAccount(user_id);
-- Appointment
ALTER TABLE dbo.Appointment
    ADD CONSTRAINT FK_Appointment_Doctor FOREIGN KEY (doctor_id)
        REFERENCES dbo.Doctor(user_id);

ALTER TABLE dbo.Appointment
    ADD CONSTRAINT FK_Appointment_Patient FOREIGN KEY (patient_id)
        REFERENCES dbo.Patient(user_id);

ALTER TABLE dbo.Appointment
    ADD CONSTRAINT CK_Appointment_Status
        CHECK (status IN (0,1,2,3));

-- Feedback
ALTER TABLE dbo.Feedback
    ADD CONSTRAINT FK_Feedback_Appointment FOREIGN KEY (appointment_id)
        REFERENCES dbo.Appointment(appointment_id);
