using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalConnect.Migrations
{
    /// <inheritdoc />
    public partial class updateDoctorsPages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WaitingTime",
                table: "DoctorAvailabilities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WaitingTime",
                table: "DoctorAvailabilities");
        }
    }
}
