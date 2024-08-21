using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AxTask.Migrations
{
    /// <inheritdoc />
    public partial class AddedSerializing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RecordValues",
                table: "LogRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecordValues",
                table: "LogRecords");
        }
    }
}
