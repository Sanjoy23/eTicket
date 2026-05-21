using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Event.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Updateconcurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
            name: "RowVersion",
            table: "EventSeatInventories");

            migrationBuilder.AlterColumn<uint>(
                name: "xmin",
                table: "EventSeatInventories",
                type: "xid",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldRowVersion: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "xmin",
                table: "EventSeatInventories",
                newName: "RowVersion");

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "EventSeatInventories",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(uint),
                oldType: "xid",
                oldRowVersion: true);
        }
    }
}
