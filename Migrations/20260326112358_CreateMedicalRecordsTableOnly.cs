using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicBookingMVC.Migrations
{
    /// <inheritdoc />
    public partial class CreateMedicalRecordsTableOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeSlots_Doctors_DoctorId",
                table: "TimeSlots");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Users__1788CC4C427F2F31",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimeSlots",
                table: "TimeSlots");

            migrationBuilder.DropIndex(
                name: "IX_TimeSlots_DoctorId",
                table: "TimeSlots");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Specialt__D768F6A86F916402",
                table: "Specialties");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Roles__8AFACE1A59CDC53F",
                table: "Roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Doctors__2DC00EBF097690A9",
                table: "Doctors");

            migrationBuilder.DropPrimaryKey(
                name: "PK__ContactM__2B0D4DFC3ADFF039",
                table: "ContactMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Appointm__C8EE206331249261",
                table: "AppointmentStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Appointm__8ECDFCC243077697",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_DoctorId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "TimeSlots");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "TimeSlots");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "TimeSlots");

            migrationBuilder.RenameIndex(
                name: "UQ__Users__A9D10534200FD58A",
                table: "Users",
                newName: "UQ__Users__A9D1053490230EFD");

            migrationBuilder.RenameIndex(
                name: "UQ__Roles__8A2B6160FF6999C8",
                table: "Roles",
                newName: "UQ__Roles__8A2B6160B93F7520");

            migrationBuilder.RenameIndex(
                name: "UQ__Appointm__05E7698A6E320F0C",
                table: "AppointmentStatuses",
                newName: "UQ__Appointm__05E7698A98D7986F");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "Appointments",
                newName: "UserPatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_PatientId",
                table: "Appointments",
                newName: "IX_Appointments_UserPatientId");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TimeSlots",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "SlotLabel",
                table: "TimeSlots",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Specialties",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Doctors",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<decimal>(
                name: "ConsultationFee",
                table: "Doctors",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Doctors",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(getdate())");

            migrationBuilder.AddColumn<string>(
                name: "Qualification",
                table: "Doctors",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsReplied",
                table: "ContactMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "BookingCode",
                table: "Appointments",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DoctorScheduleSlotId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Appointments",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScheduleId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TimeSlotId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Appointments",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK__Users__1788CC4CDD68CD99",
                table: "Users",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK__TimeSlot__41CC1F32F3A121BF",
                table: "TimeSlots",
                column: "TimeSlotId");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Specialt__D768F6A8FD3FF84C",
                table: "Specialties",
                column: "SpecialtyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Roles__8AFACE1AD3B39B06",
                table: "Roles",
                column: "RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Doctors__2DC00EBF6CC17F2B",
                table: "Doctors",
                column: "DoctorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK__ContactM__2B0D4DFCC3A37FA2",
                table: "ContactMessages",
                column: "ContactMessageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Appointm__C8EE2063FF069F87",
                table: "AppointmentStatuses",
                column: "StatusId");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Appointm__8ECDFCC2616B6ACE",
                table: "Appointments",
                column: "AppointmentId");

            migrationBuilder.CreateTable(
                name: "AppointmentStatusHistory",
                columns: table => new
                {
                    HistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<int>(type: "int", nullable: false),
                    OldStatusId = table.Column<int>(type: "int", nullable: true),
                    NewStatusId = table.Column<int>(type: "int", nullable: false),
                    ChangedByUserId = table.Column<int>(type: "int", nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Appointm__4D7B4ABD4264157D", x => x.HistoryId);
                    table.ForeignKey(
                        name: "FK_StatusHistory_Appointments",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "AppointmentId");
                    table.ForeignKey(
                        name: "FK_StatusHistory_NewStatus",
                        column: x => x.NewStatusId,
                        principalTable: "AppointmentStatuses",
                        principalColumn: "StatusId");
                    table.ForeignKey(
                        name: "FK_StatusHistory_OldStatus",
                        column: x => x.OldStatusId,
                        principalTable: "AppointmentStatuses",
                        principalColumn: "StatusId");
                    table.ForeignKey(
                        name: "FK_StatusHistory_Users",
                        column: x => x.ChangedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "DoctorSchedules",
                columns: table => new
                {
                    ScheduleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    WorkDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DoctorSc__9C8A5B49B944F90A", x => x.ScheduleId);
                    table.ForeignKey(
                        name: "FK_DoctorSchedules_Doctors",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "DoctorId");
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    PatientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EmergencyContactName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    EmergencyContactPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Patients__970EC366E68E0310", x => x.PatientId);
                    table.ForeignKey(
                        name: "FK_Patients_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TransactionCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PaidAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payments__9B556A38A928FCC6", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_Payments_Appointments",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "AppointmentId");
                });

            migrationBuilder.CreateTable(
                name: "DoctorScheduleSlots",
                columns: table => new
                {
                    DoctorScheduleSlotId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScheduleId = table.Column<int>(type: "int", nullable: false),
                    TimeSlotId = table.Column<int>(type: "int", nullable: false),
                    MaxAppointments = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CurrentAppointments = table.Column<int>(type: "int", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DoctorSc__14C4F2DE15EC6A1D", x => x.DoctorScheduleSlotId);
                    table.ForeignKey(
                        name: "FK_DoctorScheduleSlots_Schedules",
                        column: x => x.ScheduleId,
                        principalTable: "DoctorSchedules",
                        principalColumn: "ScheduleId");
                    table.ForeignKey(
                        name: "FK_DoctorScheduleSlots_TimeSlots",
                        column: x => x.TimeSlotId,
                        principalTable: "TimeSlots",
                        principalColumn: "TimeSlotId");
                });

            migrationBuilder.CreateIndex(
                name: "UQ__Specialt__7DCA57486191A721",
                table: "Specialties",
                column: "SpecialtyName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Doctors__1788CC4D28940976",
                table: "Doctors",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorScheduleSlotId",
                table: "Appointments",
                column: "DoctorScheduleSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ScheduleId",
                table: "Appointments",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_TimeSlotId",
                table: "Appointments",
                column: "TimeSlotId");

            migrationBuilder.CreateIndex(
                name: "UQ__Appointm__C6E56BD5E379392F",
                table: "Appointments",
                column: "BookingCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Appointments_Doctor_Date_Time",
                table: "Appointments",
                columns: new[] { "DoctorId", "AppointmentDate", "AppointmentTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentStatusHistory_AppointmentId",
                table: "AppointmentStatusHistory",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentStatusHistory_ChangedByUserId",
                table: "AppointmentStatusHistory",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentStatusHistory_NewStatusId",
                table: "AppointmentStatusHistory",
                column: "NewStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentStatusHistory_OldStatusId",
                table: "AppointmentStatusHistory",
                column: "OldStatusId");

            migrationBuilder.CreateIndex(
                name: "UQ_DoctorSchedules_Doctor_WorkDate",
                table: "DoctorSchedules",
                columns: new[] { "DoctorId", "WorkDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DoctorScheduleSlots_TimeSlotId",
                table: "DoctorScheduleSlots",
                column: "TimeSlotId");

            migrationBuilder.CreateIndex(
                name: "UQ_DoctorScheduleSlots",
                table: "DoctorScheduleSlots",
                columns: new[] { "ScheduleId", "TimeSlotId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Patients__1788CC4DE5D24A9E",
                table: "Patients",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_AppointmentId",
                table: "Payments",
                column: "AppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_DoctorScheduleSlots",
                table: "Appointments",
                column: "DoctorScheduleSlotId",
                principalTable: "DoctorScheduleSlots",
                principalColumn: "DoctorScheduleSlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_DoctorSchedules",
                table: "Appointments",
                column: "ScheduleId",
                principalTable: "DoctorSchedules",
                principalColumn: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_TimeSlots",
                table: "Appointments",
                column: "TimeSlotId",
                principalTable: "TimeSlots",
                principalColumn: "TimeSlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Users_UserId",
                table: "Doctors",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_DoctorScheduleSlots",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_DoctorSchedules",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_TimeSlots",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Users_UserId",
                table: "Doctors");

            migrationBuilder.DropTable(
                name: "AppointmentStatusHistory");

            migrationBuilder.DropTable(
                name: "DoctorScheduleSlots");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "DoctorSchedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Users__1788CC4CDD68CD99",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK__TimeSlot__41CC1F32F3A121BF",
                table: "TimeSlots");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Specialt__D768F6A8FD3FF84C",
                table: "Specialties");

            migrationBuilder.DropIndex(
                name: "UQ__Specialt__7DCA57486191A721",
                table: "Specialties");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Roles__8AFACE1AD3B39B06",
                table: "Roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Doctors__2DC00EBF6CC17F2B",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "UQ__Doctors__1788CC4D28940976",
                table: "Doctors");

            migrationBuilder.DropPrimaryKey(
                name: "PK__ContactM__2B0D4DFCC3A37FA2",
                table: "ContactMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Appointm__C8EE2063FF069F87",
                table: "AppointmentStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Appointm__8ECDFCC2616B6ACE",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_DoctorScheduleSlotId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_ScheduleId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_TimeSlotId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "UQ__Appointm__C6E56BD5E379392F",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "UQ_Appointments_Doctor_Date_Time",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TimeSlots");

            migrationBuilder.DropColumn(
                name: "SlotLabel",
                table: "TimeSlots");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Specialties");

            migrationBuilder.DropColumn(
                name: "ConsultationFee",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "Qualification",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "IsReplied",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "BookingCode",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "DoctorScheduleSlotId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "TimeSlotId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Appointments");

            migrationBuilder.RenameIndex(
                name: "UQ__Users__A9D1053490230EFD",
                table: "Users",
                newName: "UQ__Users__A9D10534200FD58A");

            migrationBuilder.RenameIndex(
                name: "UQ__Roles__8A2B6160B93F7520",
                table: "Roles",
                newName: "UQ__Roles__8A2B6160FF6999C8");

            migrationBuilder.RenameIndex(
                name: "UQ__Appointm__05E7698A98D7986F",
                table: "AppointmentStatuses",
                newName: "UQ__Appointm__05E7698A6E320F0C");

            migrationBuilder.RenameColumn(
                name: "UserPatientId",
                table: "Appointments",
                newName: "PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_UserPatientId",
                table: "Appointments",
                newName: "IX_Appointments_PatientId");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "TimeSlots",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DoctorId",
                table: "TimeSlots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "TimeSlots",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Doctors",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK__Users__1788CC4C427F2F31",
                table: "Users",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimeSlots",
                table: "TimeSlots",
                column: "TimeSlotId");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Specialt__D768F6A86F916402",
                table: "Specialties",
                column: "SpecialtyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Roles__8AFACE1A59CDC53F",
                table: "Roles",
                column: "RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Doctors__2DC00EBF097690A9",
                table: "Doctors",
                column: "DoctorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK__ContactM__2B0D4DFC3ADFF039",
                table: "ContactMessages",
                column: "ContactMessageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Appointm__C8EE206331249261",
                table: "AppointmentStatuses",
                column: "StatusId");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Appointm__8ECDFCC243077697",
                table: "Appointments",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlots_DoctorId",
                table: "TimeSlots",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorId",
                table: "Appointments",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSlots_Doctors_DoctorId",
                table: "TimeSlots",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "DoctorId");
        }
    }
}
