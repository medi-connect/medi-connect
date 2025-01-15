MediConnect
===========

MediConnect is a modern healthcare platform designed to bridge the gap between patients and doctors. The application allows patients to find doctors, book appointments, and provide feedback about their experience. MediConnect leverages advanced technologies like .NET Core 8, Flutter Web, Azure Cloud, and Kubernetes to deliver an efficient, scalable, and robust solution.

Table of Contents
-----------------

1.  [Overview](#1-overview)

2.  [Technologies Used](#2-technologies-used)

3.  [Architecture](#3-architecture)

4.  [Microservices and Responsibilities](#4-microservices-and-responsibilities)

5.  [Database Design](#5-database-design)

6.  [Setup and Deployment](#6-setup-and-deployment)

7.  [API Documentation](#7-api-documentation)

8.  [Development and Contribution Guidelines](#8-development-and-contribution-guidelines)



1\. Overview
------------

MediConnect facilitates patient-doctor interactions by providing features such as appointment scheduling, doctor search, and user feedback. The app consists of:

*   **Frontend**: A Flutter Web-based user interface for a good UI/UX.

*   **Backend**: A microservices architecture built on .NET Core 8, divided into four microservices.

*   **Deployment**: The application is containerized using Docker and deployed on Kubernetes (K8s) on Azure Cloud.


2\. Technologies Used
---------------------

### Backend

*   **Programming Language**: .NET Core 8

*   **Microservices**: UserService, AppointmentService, DoctorService, FeedbackService

*   **Database**: SQL Server

*   **Containerization**: Docker

*   **Orchestration**: Kubernetes (K8s)


### Frontend

*   **Framework**: Flutter Web


### Hosting and Deployment

*   **Cloud Provider**: Azure

*   **CI/CD Pipeline**: GitHub Actions


3\. Architecture
----------------

MediConnect follows a **microservices architecture** for decoupling the application and quick scaling. Each microservice is self-contained and has its own database schema to adhere to the **database-per-service** principle.

*   **Frontend**: Flutter web application.

*   **Backend**: RESTful APIs exposed by independent microservices.

*   **Database**: Each microservice has its own table(s) in the SQL Server database.

*   **Communication**:

    *   **Synchronous communication**: REST APIs (HTTP) are used to communicate between the client/frontend and microservices.

    *   **Asynchronous communication** (future scope): To enable scalability, we consider integrating message queues like RabbitMQ.


4\. Microservices and Responsibilities
--------------------------------------

### 1\. **UserService**

*   Manage user registration and login.

*   Manage patient registration, login and business logic.

*   Use JWT for secure user authentication.

*   Key Endpoints:

    *   POST /api/v1/user/login

    *   POST /api/v1/user/register


### 2\. **AppointmentService**

*   Enable patients to book, retrieve, and manage appointments with doctors.

*   Key Endpoints:

    *   POST /api/v1/appointment/createAppointment

    *   GET /api/v1/appointment/getAppointmentsForPatient/{patientId}

    *   GET /api/v1/appointment/getAppointmentsForDoctor/{doctorId}


### 3\. **DoctorService**

*   Manage doctor profiles, specialties, and availability.

*   Integrate search functionality to allow patients to find the right doctor.

*   Key Endpoints:

    *   PUT /api/v1/doctor/updateDoctor/{id}

    *   GET /api/v1/doctor/getDoctor/{id}

    *   GET /api/v1/doctor/getDoctorsBySpeciality/{specialty}


### 4\. **FeedbackService**

*   Collect feedback from patients about their appointments and doctors.

*   Store and manage feedback in the database.

*   Key Endpoints:

    *   POST /api/feedback/addFeedback

    *   GET /api/feedback/getFeedback/{feedbackId}


# 5. Database Design

Each microservice is designed to manage its own database schema, following the **database-per-service** pattern. This approach ensures better scalability, loose coupling, and independent maintainability of microservices.

Below is a description of the database tables for each microservice:

---

### 1. UserService

Handles the user accounts, authentication, and roles for the system. This includes both doctors and patients.

#### UserAccount Table:
- `UserId` (PK): Unique identifier for the user.
- `Email` (Unique, Not Null): User’s email address.
- `Password` (Not Null): Hashed password for authentication.
- `Status` (Not Null): Active/inactive status of the user.
- `IsDoctor` (Not Null): Indicates whether the user is a doctor (`1`) or a patient (`0`).
- `Sys_Timestamp`: Last updated timestamp.
- `Sys_Created`: Record creation timestamp.

**Relationships**:
- Doctors and Patients have a one-to-one relationship mapped through the `UserId` foreign key in the `Doctor` and `Patient` tables.

---

### 2. AppointmentService

Manages appointment scheduling and tracking between doctors and patients.

#### Appointment Table:
- `AppointmentId` (PK): Unique identifier for the appointment.
- `Start_Time` (Not Null): Date and time when the appointment starts.
- `End_Time`: Date and time when the appointment ends.
- `Title` (Not Null): Title or subject of the appointment.
- `Description`: Brief description of the appointment or notes.
- `Status`: Current status (`0 - Pending`, `1 - Confirmed`, `2 - Completed`, `3 - Cancelled`).
- `DoctorId` (FK): Refers to the `Doctor.UserId` for the doctor assigned to the appointment.
- `PatientId` (FK): Refers to the `Patient.UserId` for the patient who scheduled the appointment.
- `Created_By`: Indicates who created the appointment (`0 - Patient`, `1 - Doctor`).
- `Sys_Timestamp`: Last updated timestamp.
- `Sys_Created`: Record creation timestamp.

**Constraints**:
- `FK_Appointment_Doctor`: References the `Doctor.UserId` for the assigned doctor.
- `FK_Appointment_Patient`: References the `Patient.UserId` for the patient.
- `CK_Appointment_Status`: Ensures `Status` can only be `0`, `1`, `2`, or `3`.

---

### 3. DoctorService

Manages information related to doctors and their specialties.

#### Doctor Table:
- `UserId` (PK, FK): Links to the `UserAccount.UserId`.
- `Name` (Not Null): Doctor’s first name.
- `Surname` (Not Null): Doctor’s last name.
- `Speciality`: Doctor’s area of specialization (e.g., cardiology, dermatology).
- `Sys_Timestamp`: Last updated timestamp.
- `Sys_Created`: Record creation timestamp.

**Relationships**:
- `FK_Doctor_User`: References the `UserAccount.UserId` to link general user data with doctor-specific data.

---

### 4. PatientService

Manages detailed information about patients.

#### Patient Table:
- `UserId` (PK, FK): Links to the `UserAccount.UserId`.
- `Name` (Not Null): Patient’s first name.
- `Surname` (Not Null): Patient’s last name.
- `Birth_Date` (Not Null): Patient’s date of birth.
- `Sys_Timestamp`: Last updated timestamp.
- `Sys_Created`: Record creation timestamp.

**Relationships**:
- `FK_Patient_User`: References the `UserAccount.UserId` to link general user data with patient-specific data.

---

### 5. FeedbackService

Handles ratings and reviews provided by patients for their appointments with doctors.

#### Feedback Table:
- `FeedbackId` (PK): Unique identifier for the feedback.
- `Rate` (Not Null): Rating provided by the patient (e.g., between 1-5).
- `Review` (Not Null): Written review by the patient about the doctor or appointment.
- `AppointmentId` (FK, Not Null): Links to the `Appointment.AppointmentId` to associate feedback with a specific appointment.
- `Sys_Timestamp`: Last updated timestamp.
- `Sys_Created`: Record creation timestamp.

**Constraints**:
- `FK_Feedback_Appointment`: References the `Appointment.AppointmentId` to associate feedback with the correct appointment.

6\. Setup and Deployment
------------------------

### Prerequisites

*   .NET Core 8 SDK

*   Flutter SDK

*   SQL Server instance

*   Docker and Kubernetes CLI tools (e.g., kubectl)

*   Azure account with Kubernetes Service (AKS)


### Steps to Run Locally

#### Backend (Microservices)

*   git clone https://github.com/medi-connect/medi-connect.git


1.  cd api/UserService 
  - dotnet restore
  - dotnet run

2.  Repeat for AppointmentService, DoctorService, and FeedbackService.


#### Frontend

1.  bash

2.  cd flutter-webflutter pub getflutter run -d chrome


#### Database

1.  Set up the SQL Server database using the scripts provided in the database/scripts folder.

#### Kubernetes Deployment

1.  cd api/UserService
2.  kubectl apply -f user-deployment.yaml
3.  Repeat for AppointmentService, DoctorService, and FeedbackService.



7\. API Documentation
---------------------

For detailed API specifications, see [API Specification](https://github.com/medi-connect/medi-connect/blob/dev/docs/APISpecification.md).

8\. Development and Contribution Guidelines
-------------------------------------------

### Code Standards

*   Use consistent code formatting across all microservices.

*   Enforce API versioning (e.g., /api/v1/...).


### Branching Strategy

*   Follow GitFlow branching model:

    *   main: Always stable and deployable.

    *   dev: Feature branches merged here after code review.


Future Considerations
---------------------

*   Implement caching mechanisms for frequently accessed endpoints (e.g., Redis).

*   Add asynchronous messaging using Azure Service Bus or RabbitMQ.


Authors
-------

*   **Nikola Srebrov**

*   Contact: [ns06802@student.uni-lj.si](mailto:ns06802@student.uni-lj.si)

*   **Makedonka Binova**

*   Contact: [mb24729@student.uni-lj.si](mailto:mb24729@student.uni-lj.si)