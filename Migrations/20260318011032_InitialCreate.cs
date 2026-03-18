using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicBookingMVC.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // The tables already exist in the database from manual creation or previous seeds
            // Uncomment this if recreating a fresh database:
            /*
            migrationBuilder.CreateTable(
                name: "AppointmentStatuses",
... (rest of the creation script commented out for sync purpose) ...
            */
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Commented out to avoid dropping existing tables accidentally
            /*
            migrationBuilder.DropTable(
                name: "ContactMessages");
... (rest of the down script commented out) ...
            */
        }
    }
}
