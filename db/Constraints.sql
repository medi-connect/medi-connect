-- Doctor
ALTER TABLE dbo.Doctor
    ADD CONSTRAINT FK_Doctor_User FOREIGN KEY (id)
        REFERENCES dbo.UserAccount(id);

-- Patient
ALTER TABLE dbo.Patient
    ADD CONSTRAINT FK_Patient_User FOREIGN KEY (id)
        REFERENCES dbo.UserAccount(id);
-- Appointment
ALTER TABLE dbo.Appointment
    ADD CONSTRAINT FK_Appointment_Doctor FOREIGN KEY (doctor_id)
        REFERENCES dbo.Doctor(id);

ALTER TABLE dbo.Appointment
    ADD CONSTRAINT FK_Appointment_Patient FOREIGN KEY (patient_id)
        REFERENCES dbo.Patient(id);

-- Feedback
ALTER TABLE dbo.Feedback
    ADD CONSTRAINT FK_Feedback_Appointment FOREIGN KEY (id)
        REFERENCES dbo.Appointment(id);
