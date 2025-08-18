using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumeFilterProject.Migrations
{
    /// <inheritdoc />
    public partial class Resume : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ParsedText",
                table: "ResumeLabels",
                newName: "Skills");

            migrationBuilder.RenameColumn(
                name: "Labels",
                table: "ResumeLabels",
                newName: "Projects");

            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "ResumeLabels",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "Contact",
                table: "ResumeLabels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Education",
                table: "ResumeLabels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Experience",
                table: "ResumeLabels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Contact",
                table: "ResumeLabels");

            migrationBuilder.DropColumn(
                name: "Education",
                table: "ResumeLabels");

            migrationBuilder.DropColumn(
                name: "Experience",
                table: "ResumeLabels");

            migrationBuilder.RenameColumn(
                name: "Skills",
                table: "ResumeLabels",
                newName: "ParsedText");

            migrationBuilder.RenameColumn(
                name: "Projects",
                table: "ResumeLabels",
                newName: "Labels");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ResumeLabels",
                newName: "FilePath");
        }
    }
}
