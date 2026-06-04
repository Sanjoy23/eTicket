using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Booking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReceiptTableremovesomefields2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Receipts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReceiptNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PaymentType = table.Column<int>(type: "integer", nullable: false),
                    ChequeNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PaymentInfo = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    payment_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CurrencyId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PaperNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AmountString = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    TransactionResultText = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    QrCodeContentString = table.Column<string>(type: "text", nullable: false),
                    QrCodeContent = table.Column<byte[]>(type: "bytea", nullable: false),
                    IsPaid = table.Column<bool>(type: "boolean", nullable: false),
                    ModifiedIp = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedIp = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    GatewayPageUrl = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_EventId",
                table: "Receipts",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_IsPaid",
                table: "Receipts",
                column: "IsPaid");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_ReceiptNumber",
                table: "Receipts",
                column: "ReceiptNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_TransactionId",
                table: "Receipts",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_UserId",
                table: "Receipts",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Receipts");
        }
    }
}
