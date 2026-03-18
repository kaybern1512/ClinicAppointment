using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicBookingMVC.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDoctorUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // The column was already removed manually, but we drop it here to be safe if it exists
            /*
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Doctors");
            */
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.AddColumn<int>(
            //     name: "UserId",
            //     table: "Doctors",
            //     type: "int",
            //     nullable: false,
            //     defaultValue: 0);
        }
    }
}
