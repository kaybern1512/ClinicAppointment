CREATE DATABASE ClinicBookingDB;
GO

USE ClinicBookingDB;
GO

/* =========================================================
   DROP OLD TABLES IF EXISTS (for rerun during development)
========================================================= */
IF OBJECT_ID('Payments', 'U') IS NOT NULL DROP TABLE Payments;
IF OBJECT_ID('AppointmentStatusHistory', 'U') IS NOT NULL DROP TABLE AppointmentStatusHistory;
IF OBJECT_ID('Appointments', 'U') IS NOT NULL DROP TABLE Appointments;
IF OBJECT_ID('DoctorScheduleSlots', 'U') IS NOT NULL DROP TABLE DoctorScheduleSlots;
IF OBJECT_ID('DoctorSchedules', 'U') IS NOT NULL DROP TABLE DoctorSchedules;
IF OBJECT_ID('TimeSlots', 'U') IS NOT NULL DROP TABLE TimeSlots;
IF OBJECT_ID('Doctors', 'U') IS NOT NULL DROP TABLE Doctors;
IF OBJECT_ID('Patients', 'U') IS NOT NULL DROP TABLE Patients;
IF OBJECT_ID('ContactMessages', 'U') IS NOT NULL DROP TABLE ContactMessages;
IF OBJECT_ID('AppointmentStatuses', 'U') IS NOT NULL DROP TABLE AppointmentStatuses;
IF OBJECT_ID('Specialties', 'U') IS NOT NULL DROP TABLE Specialties;
IF OBJECT_ID('Users', 'U') IS NOT NULL DROP TABLE Users;
IF OBJECT_ID('Roles', 'U') IS NOT NULL DROP TABLE Roles;
GO

/* =========================================================
   1. Roles
========================================================= */
CREATE TABLE Roles
(
    RoleId INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL UNIQUE
);
GO

/* =========================================================
   2. Users
========================================================= */
CREATE TABLE Users
(
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(150) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    PhoneNumber NVARCHAR(20) NOT NULL,
    [Password] NVARCHAR(255) NOT NULL,
    RoleId INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsActive BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(RoleId)
);
GO

/* =========================================================
   3. Patients
   Tách riêng thông tin bệnh nhân mở rộng
========================================================= */
CREATE TABLE Patients
(
    PatientId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL UNIQUE,
    DateOfBirth DATE NULL,
    Gender NVARCHAR(20) NULL,
    Address NVARCHAR(255) NULL,
    EmergencyContactName NVARCHAR(150) NULL,
    EmergencyContactPhone NVARCHAR(20) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Patients_Users FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
GO

/* =========================================================
   4. Specialties
========================================================= */
CREATE TABLE Specialties
(
    SpecialtyId INT IDENTITY(1,1) PRIMARY KEY,
    SpecialtyName NVARCHAR(150) NOT NULL UNIQUE,
    [Description] NVARCHAR(1000) NULL,
    Icon NVARCHAR(100) NULL,
    ImageUrl NVARCHAR(255) NULL,
    IsFeatured BIT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1
);
GO

/* =========================================================
   5. Doctors
   Bác sĩ có thể có tài khoản đăng nhập -> UserId
========================================================= */
CREATE TABLE Doctors
(
    DoctorId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NULL UNIQUE,
    FullName NVARCHAR(150) NOT NULL,
    SpecialtyId INT NOT NULL,
    ExperienceYears INT NOT NULL CHECK (ExperienceYears >= 0),
    [Description] NVARCHAR(2000) NULL,
    Qualification NVARCHAR(255) NULL,
    ImageUrl NVARCHAR(255) NULL,
    WorkingTime NVARCHAR(255) NULL,
    ConsultationFee DECIMAL(18,2) NOT NULL DEFAULT 0,
    IsFeatured BIT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Doctors_Users FOREIGN KEY (UserId) REFERENCES Users(UserId),
    CONSTRAINT FK_Doctors_Specialties FOREIGN KEY (SpecialtyId) REFERENCES Specialties(SpecialtyId)
);
GO

/* =========================================================
   6. Appointment Statuses
========================================================= */
CREATE TABLE AppointmentStatuses
(
    StatusId INT IDENTITY(1,1) PRIMARY KEY,
    StatusName NVARCHAR(50) NOT NULL UNIQUE
);
GO

/* =========================================================
   7. TimeSlots
   Danh sách khung giờ dùng chung
========================================================= */
CREATE TABLE TimeSlots
(
    TimeSlotId INT IDENTITY(1,1) PRIMARY KEY,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    SlotLabel NVARCHAR(50) NULL,
    IsActive BIT NOT NULL DEFAULT 1,

    CONSTRAINT CK_TimeSlots_Valid CHECK (StartTime < EndTime)
);
GO

/* =========================================================
   8. DoctorSchedules
   Lịch làm việc theo ngày của bác sĩ
========================================================= */
CREATE TABLE DoctorSchedules
(
    ScheduleId INT IDENTITY(1,1) PRIMARY KEY,
    DoctorId INT NOT NULL,
    WorkDate DATE NOT NULL,
    Notes NVARCHAR(500) NULL,
    IsAvailable BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_DoctorSchedules_Doctors FOREIGN KEY (DoctorId) REFERENCES Doctors(DoctorId),
    CONSTRAINT UQ_DoctorSchedules_Doctor_WorkDate UNIQUE (DoctorId, WorkDate)
);
GO

/* =========================================================
   9. DoctorScheduleSlots
   Slot cụ thể trong từng ngày của bác sĩ
========================================================= */
CREATE TABLE DoctorScheduleSlots
(
    DoctorScheduleSlotId INT IDENTITY(1,1) PRIMARY KEY,
    ScheduleId INT NOT NULL,
    TimeSlotId INT NOT NULL,
    MaxAppointments INT NOT NULL DEFAULT 1 CHECK (MaxAppointments > 0),
    CurrentAppointments INT NOT NULL DEFAULT 0 CHECK (CurrentAppointments >= 0),
    IsAvailable BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_DoctorScheduleSlots_Schedules FOREIGN KEY (ScheduleId) REFERENCES DoctorSchedules(ScheduleId),
    CONSTRAINT FK_DoctorScheduleSlots_TimeSlots FOREIGN KEY (TimeSlotId) REFERENCES TimeSlots(TimeSlotId),
    CONSTRAINT UQ_DoctorScheduleSlots UNIQUE (ScheduleId, TimeSlotId)
);
GO

/* =========================================================
   10. Appointments
   Đặt lịch theo schedule slot
========================================================= */
CREATE TABLE Appointments
(
    AppointmentId INT IDENTITY(1,1) PRIMARY KEY,

    -- account patient nếu có
    UserPatientId INT NULL,

    -- snapshot thông tin lúc đặt lịch
    PatientName NVARCHAR(150) NOT NULL,
    PhoneNumber NVARCHAR(20) NOT NULL,
    Email NVARCHAR(150) NOT NULL,
    DateOfBirth DATE NULL,
    Gender NVARCHAR(20) NULL,

    DoctorId INT NOT NULL,
    SpecialtyId INT NOT NULL,
    ScheduleId INT NOT NULL,
    TimeSlotId INT NOT NULL,
    DoctorScheduleSlotId INT NOT NULL,

    AppointmentDate DATE NOT NULL,
    AppointmentTime TIME NOT NULL,

    Symptoms NVARCHAR(1000) NULL,
    Note NVARCHAR(1000) NULL,
    StatusId INT NOT NULL,
    BookingCode NVARCHAR(30) NOT NULL UNIQUE,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,

    CONSTRAINT FK_Appointments_Users FOREIGN KEY (UserPatientId) REFERENCES Users(UserId),
    CONSTRAINT FK_Appointments_Doctors FOREIGN KEY (DoctorId) REFERENCES Doctors(DoctorId),
    CONSTRAINT FK_Appointments_Specialties FOREIGN KEY (SpecialtyId) REFERENCES Specialties(SpecialtyId),
    CONSTRAINT FK_Appointments_DoctorSchedules FOREIGN KEY (ScheduleId) REFERENCES DoctorSchedules(ScheduleId),
    CONSTRAINT FK_Appointments_TimeSlots FOREIGN KEY (TimeSlotId) REFERENCES TimeSlots(TimeSlotId),
    CONSTRAINT FK_Appointments_DoctorScheduleSlots FOREIGN KEY (DoctorScheduleSlotId) REFERENCES DoctorScheduleSlots(DoctorScheduleSlotId),
    CONSTRAINT FK_Appointments_AppointmentStatuses FOREIGN KEY (StatusId) REFERENCES AppointmentStatuses(StatusId)
);
GO

/* =========================================================
   Chống trùng lịch bác sĩ cùng ngày cùng giờ
========================================================= */
ALTER TABLE Appointments
ADD CONSTRAINT UQ_Appointments_Doctor_Date_Time UNIQUE (DoctorId, AppointmentDate, AppointmentTime);
GO

/* =========================================================
   11. AppointmentStatusHistory
========================================================= */
CREATE TABLE AppointmentStatusHistory
(
    HistoryId INT IDENTITY(1,1) PRIMARY KEY,
    AppointmentId INT NOT NULL,
    OldStatusId INT NULL,
    NewStatusId INT NOT NULL,
    ChangedByUserId INT NULL,
    ChangedAt DATETIME NOT NULL DEFAULT GETDATE(),
    Note NVARCHAR(500) NULL,

    CONSTRAINT FK_StatusHistory_Appointments FOREIGN KEY (AppointmentId) REFERENCES Appointments(AppointmentId),
    CONSTRAINT FK_StatusHistory_OldStatus FOREIGN KEY (OldStatusId) REFERENCES AppointmentStatuses(StatusId),
    CONSTRAINT FK_StatusHistory_NewStatus FOREIGN KEY (NewStatusId) REFERENCES AppointmentStatuses(StatusId),
    CONSTRAINT FK_StatusHistory_Users FOREIGN KEY (ChangedByUserId) REFERENCES Users(UserId)
);
GO

/* =========================================================
   12. Payments
========================================================= */
CREATE TABLE Payments
(
    PaymentId INT IDENTITY(1,1) PRIMARY KEY,
    AppointmentId INT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL CHECK (Amount >= 0),
    PaymentMethod NVARCHAR(50) NOT NULL,
    PaymentStatus NVARCHAR(50) NOT NULL,
    TransactionCode NVARCHAR(100) NULL,
    PaidAt DATETIME NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Payments_Appointments FOREIGN KEY (AppointmentId) REFERENCES Appointments(AppointmentId)
);
GO

/* =========================================================
   13. ContactMessages
========================================================= */
CREATE TABLE ContactMessages
(
    ContactMessageId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(150) NOT NULL,
    Email NVARCHAR(150) NOT NULL,
    Message NVARCHAR(2000) NOT NULL,
    SentAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsReplied BIT NOT NULL DEFAULT 0
);
GO

/* =========================================================
   SEED DATA
========================================================= */

/* Roles */
INSERT INTO Roles (RoleName)
VALUES 
(N'Admin'),
(N'Patient'),
(N'Doctor');
GO

/* Users */
INSERT INTO Users (FullName, Email, PhoneNumber, [Password], RoleId, CreatedAt, IsActive)
VALUES
(N'Nguyễn Văn Admin', 'admin@clinic.com', '0900000001', '123456', 1, GETDATE(), 1),

(N'Trần Thị Lan', 'lan@gmail.com', '0900000002', '123456', 2, GETDATE(), 1),
(N'Lê Minh Khoa', 'khoa@gmail.com', '0900000003', '123456', 2, GETDATE(), 1),
(N'Phạm Thu Hà', 'ha@gmail.com', '0900000004', '123456', 2, GETDATE(), 1),

(N'BS. Nguyễn Hoàng Anh', 'doctor1@clinic.com', '0901000001', '123456', 3, GETDATE(), 1),
(N'BS. Trần Thu Mai', 'doctor2@clinic.com', '0901000002', '123456', 3, GETDATE(), 1),
(N'BS. Lê Minh Đức', 'doctor3@clinic.com', '0901000003', '123456', 3, GETDATE(), 1),
(N'BS. Phạm Ngọc Hân', 'doctor4@clinic.com', '0901000004', '123456', 3, GETDATE(), 1),
(N'BS. Đặng Quốc Bảo', 'doctor5@clinic.com', '0901000005', '123456', 3, GETDATE(), 1),
(N'BS. Vũ Thảo Nhi', 'doctor6@clinic.com', '0901000006', '123456', 3, GETDATE(), 1);
GO

/* Patients */
INSERT INTO Patients (UserId, DateOfBirth, Gender, Address, EmergencyContactName, EmergencyContactPhone)
VALUES
(2, '2002-04-15', N'Nữ', N'Đà Nẵng', N'Mẹ Trần Thị Lan', '0911111111'),
(3, '2001-08-20', N'Nam', N'Quảng Nam', N'Anh trai Lê Minh Khoa', '0922222222'),
(4, '2003-01-10', N'Nữ', N'Hội An', N'Bố Phạm Thu Hà', '0933333333');
GO

/* Specialties */
INSERT INTO Specialties (SpecialtyName, [Description], Icon, ImageUrl, IsFeatured, IsActive)
VALUES
(N'Nội tổng quát', N'Khám và điều trị các bệnh lý nội khoa phổ biến.', 'bi-heart-pulse', '/images/specialties/internal.jpg', 1, 1),
(N'Nhi khoa', N'Chăm sóc sức khỏe cho trẻ em.', 'bi-emoji-smile', '/images/specialties/pediatrics.jpg', 1, 1),
(N'Tim mạch', N'Khám và tư vấn các vấn đề tim mạch.', 'bi-heart', '/images/specialties/cardiology.jpg', 1, 1),
(N'Da liễu', N'Khám và điều trị các bệnh về da.', 'bi-droplet', '/images/specialties/dermatology.jpg', 0, 1),
(N'Tai Mũi Họng', N'Khám chuyên sâu tai mũi họng.', 'bi-ear', '/images/specialties/ent.jpg', 0, 1),
(N'Nha khoa', N'Chăm sóc và điều trị răng miệng.', 'bi-emoji-laughing', '/images/specialties/dental.jpg', 1, 1);
GO

/* Doctors */
INSERT INTO Doctors
(UserId, FullName, SpecialtyId, ExperienceYears, [Description], Qualification, ImageUrl, WorkingTime, ConsultationFee, IsFeatured, IsActive, CreatedAt)
VALUES
(5, N'BS. Nguyễn Hoàng Anh', 1, 10, N'Bác sĩ nội tổng quát giàu kinh nghiệm, tận tâm với bệnh nhân.', N'Bác sĩ CKI Nội tổng quát', '/images/doctors/doctor1.jpg', N'T2 - T7: 07:30 - 16:30', 200000, 1, 1, GETDATE()),
(6, N'BS. Trần Thu Mai', 2, 8, N'Chuyên gia nhi khoa, tư vấn chăm sóc trẻ em hiệu quả.', N'Bác sĩ Nhi khoa', '/images/doctors/doctor2.jpg', N'T2 - T6: 08:00 - 17:00', 180000, 1, 1, GETDATE()),
(7, N'BS. Lê Minh Đức', 3, 12, N'Chuyên gia tim mạch với nhiều năm kinh nghiệm thực tế.', N'Thạc sĩ Tim mạch', '/images/doctors/doctor3.jpg', N'T2 - T7: 07:00 - 15:30', 250000, 1, 1, GETDATE()),
(8, N'BS. Phạm Ngọc Hân', 4, 7, N'Điều trị các bệnh da liễu phổ biến và chuyên sâu.', N'Bác sĩ Da liễu', '/images/doctors/doctor4.jpg', N'T2 - T6: 09:00 - 17:00', 180000, 0, 1, GETDATE()),
(9, N'BS. Đặng Quốc Bảo', 5, 9, N'Chuyên khoa tai mũi họng với lịch khám linh hoạt.', N'Bác sĩ Tai Mũi Họng', '/images/doctors/doctor5.jpg', N'T2 - CN: 08:00 - 20:00', 220000, 0, 1, GETDATE()),
(10, N'BS. Vũ Thảo Nhi', 6, 6, N'Bác sĩ nha khoa nhẹ nhàng, tư vấn kỹ càng.', N'Bác sĩ Răng Hàm Mặt', '/images/doctors/doctor6.jpg', N'T2 - T7: 08:00 - 18:00', 200000, 1, 1, GETDATE());
GO

/* Appointment Statuses */
INSERT INTO AppointmentStatuses (StatusName)
VALUES
(N'Pending'),
(N'Confirmed'),
(N'Completed'),
(N'Cancelled');
GO

/* TimeSlots */
INSERT INTO TimeSlots (StartTime, EndTime, SlotLabel, IsActive)
VALUES
('07:30:00', '08:00:00', N'07:30 - 08:00', 1),
('08:00:00', '08:30:00', N'08:00 - 08:30', 1),
('08:30:00', '09:00:00', N'08:30 - 09:00', 1),
('09:00:00', '09:30:00', N'09:00 - 09:30', 1),
('09:30:00', '10:00:00', N'09:30 - 10:00', 1),
('10:00:00', '10:30:00', N'10:00 - 10:30', 1),
('10:30:00', '11:00:00', N'10:30 - 11:00', 1),
('14:00:00', '14:30:00', N'14:00 - 14:30', 1),
('14:30:00', '15:00:00', N'14:30 - 15:00', 1),
('15:00:00', '15:30:00', N'15:00 - 15:30', 1);
GO

/* DoctorSchedules */
INSERT INTO DoctorSchedules (DoctorId, WorkDate, Notes, IsAvailable, CreatedAt)
VALUES
(1, '2026-03-26', N'Lịch sáng và chiều', 1, GETDATE()),
(1, '2026-03-27', N'Lịch khám thường', 1, GETDATE()),
(2, '2026-03-26', N'Lịch khám nhi', 1, GETDATE()),
(3, '2026-03-26', N'Lịch tim mạch', 1, GETDATE()),
(6, '2026-03-27', N'Lịch nha khoa', 1, GETDATE());
GO

/* DoctorScheduleSlots */
INSERT INTO DoctorScheduleSlots (ScheduleId, TimeSlotId, MaxAppointments, CurrentAppointments, IsAvailable)
VALUES
(1, 1, 1, 0, 1),
(1, 2, 1, 0, 1),
(1, 3, 1, 1, 1),
(1, 4, 1, 0, 1),
(1, 8, 1, 0, 1),

(2, 1, 1, 0, 1),
(2, 2, 1, 0, 1),
(2, 3, 1, 0, 1),

(3, 2, 1, 0, 1),
(3, 3, 1, 0, 1),
(3, 4, 1, 1, 1),

(4, 2, 1, 1, 1),
(4, 3, 1, 0, 1),

(5, 8, 1, 0, 1),
(5, 9, 1, 0, 1);
GO

/* Appointments */
INSERT INTO Appointments
(
    UserPatientId, PatientName, PhoneNumber, Email, DateOfBirth, Gender,
    DoctorId, SpecialtyId, ScheduleId, TimeSlotId, DoctorScheduleSlotId,
    AppointmentDate, AppointmentTime, Symptoms, Note, StatusId, BookingCode, CreatedAt, UpdatedAt
)
VALUES
(
    2, N'Trần Thị Lan', '0900000002', 'lan@gmail.com', '2002-04-15', N'Nữ',
    1, 1, 1, 3, 3,
    '2026-03-26', '08:30:00', N'Đau đầu và mệt mỏi kéo dài.', N'Khám buổi sáng', 1, 'BK000001', GETDATE(), NULL
),
(
    3, N'Lê Minh Khoa', '0900000003', 'khoa@gmail.com', '2001-08-20', N'Nam',
    3, 3, 4, 2, 12,
    '2026-03-26', '08:00:00', N'Tim đập nhanh, khó thở nhẹ.', N'Cần tư vấn chuyên sâu', 2, 'BK000002', GETDATE(), GETDATE()
),
(
    4, N'Phạm Thu Hà', '0900000004', 'ha@gmail.com', '2003-01-10', N'Nữ',
    2, 2, 3, 4, 11,
    '2026-03-26', '09:00:00', N'Trẻ sốt và ho.', N'Đã xác nhận', 3, 'BK000003', GETDATE(), GETDATE()
),
(
    2, N'Trần Thị Lan', '0900000002', 'lan@gmail.com', '2002-04-15', N'Nữ',
    6, 6, 5, 8, 14,
    '2026-03-27', '14:00:00', N'Đau răng hàm dưới.', N'Đã hủy', 4, 'BK000004', GETDATE(), GETDATE()
);
GO

/* AppointmentStatusHistory */
INSERT INTO AppointmentStatusHistory (AppointmentId, OldStatusId, NewStatusId, ChangedByUserId, ChangedAt, Note)
VALUES
(1, NULL, 1, 2, GETDATE(), N'Tạo lịch hẹn'),
(2, NULL, 1, 3, GETDATE(), N'Tạo lịch hẹn'),
(2, 1, 2, 1, GETDATE(), N'Admin xác nhận lịch'),
(3, NULL, 1, 4, GETDATE(), N'Tạo lịch hẹn'),
(3, 1, 2, 1, GETDATE(), N'Admin xác nhận'),
(3, 2, 3, 6, GETDATE(), N'Bác sĩ hoàn thành khám'),
(4, NULL, 1, 2, GETDATE(), N'Tạo lịch hẹn'),
(4, 1, 4, 2, GETDATE(), N'Bệnh nhân hủy lịch');
GO

/* Payments */
INSERT INTO Payments (AppointmentId, Amount, PaymentMethod, PaymentStatus, TransactionCode, PaidAt, CreatedAt)
VALUES
(1, 200000, N'Cash', N'Pending', NULL, NULL, GETDATE()),
(2, 250000, N'VNPay', N'Paid', N'VNPAY0002', GETDATE(), GETDATE()),
(3, 180000, N'Cash', N'Paid', N'CASH0003', GETDATE(), GETDATE()),
(4, 200000, N'VNPay', N'Refunded', N'VNPAY0004', GETDATE(), GETDATE());
GO

/* ContactMessages */
INSERT INTO ContactMessages (FullName, Email, Message, SentAt, IsReplied)
VALUES
(N'Nguyễn Phúc', 'phuc@gmail.com', N'Tôi muốn hỏi giờ làm việc cuối tuần.', GETDATE(), 0),
(N'Trần Mỹ Linh', 'linh@gmail.com', N'Phòng khám có khám nhi vào chủ nhật không?', GETDATE(), 0),
(N'Lê Anh Tuấn', 'tuan@gmail.com', N'Tôi cần tư vấn chuyên khoa tim mạch.', GETDATE(), 1),
(N'Phạm Thảo', 'thao@gmail.com', N'Tôi muốn đổi lịch hẹn đã đặt.', GETDATE(), 0);
GO