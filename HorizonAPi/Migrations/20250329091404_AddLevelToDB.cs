using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HorizonAPi.Migrations
{
    /// <inheritdoc />
    public partial class AddLevelToDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QualifyLevel",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QualifyLevel",
                table: "Jobs");
        }
    }
}
