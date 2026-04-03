using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddTokenUsageModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokenUsed",
                table: "Message");

            migrationBuilder.CreateTable(
                name: "TokenUsage",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MessageId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CachedContentTokenCount = table.Column<int>(type: "int", nullable: true),
                    CandidatesTokenCount = table.Column<int>(type: "int", nullable: true),
                    PromptTokenCount = table.Column<int>(type: "int", nullable: true),
                    ThoughtsTokenCount = table.Column<int>(type: "int", nullable: true),
                    ToolUsePromptTokenCount = table.Column<int>(type: "int", nullable: true),
                    TotalTokenCount = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenUsage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TokenUsage_Message_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Message",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TokenUsage_MessageId",
                table: "TokenUsage",
                column: "MessageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TokenUsage");

            migrationBuilder.AddColumn<int>(
                name: "TokenUsed",
                table: "Message",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
