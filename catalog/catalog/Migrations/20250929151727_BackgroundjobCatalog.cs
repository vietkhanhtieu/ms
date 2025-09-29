using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace catalog.Migrations
{
    /// <inheritdoc />
    public partial class BackgroundjobCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_integrationEventLogEntries",
                table: "integrationEventLogEntries");

            migrationBuilder.RenameTable(
                name: "integrationEventLogEntries",
                newName: "IntegrationEventLogEntries");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IntegrationEventLogEntries",
                table: "IntegrationEventLogEntries",
                column: "EventId");

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NextRunTime = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobSchedules",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    JobId = table.Column<string>(type: "text", nullable: false),
                    Interval = table.Column<int>(type: "integer", nullable: false),
                    JobInternalType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobSchedules_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobSchedules_JobId",
                table: "JobSchedules",
                column: "JobId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobSchedules");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IntegrationEventLogEntries",
                table: "IntegrationEventLogEntries");

            migrationBuilder.RenameTable(
                name: "IntegrationEventLogEntries",
                newName: "integrationEventLogEntries");

            migrationBuilder.AddPrimaryKey(
                name: "PK_integrationEventLogEntries",
                table: "integrationEventLogEntries",
                column: "EventId");
        }
    }
}
