using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addedCugPhone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "EmployeeContacts",
                newName: "WorkPhone");

            migrationBuilder.AddColumn<string>(
                name: "PersonalPhone",
                table: "EmployeeContacts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonalPhone",
                table: "EmployeeContacts");

            migrationBuilder.RenameColumn(
                name: "WorkPhone",
                table: "EmployeeContacts",
                newName: "Phone");
        }
    }
}
