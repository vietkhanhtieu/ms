using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleJob.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDbTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_TaskSchedules_TaskScheduleId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_TaskScheduleId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "TaskScheduleId",
                table: "Jobs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TaskScheduleId",
                table: "Jobs",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_TaskScheduleId",
                table: "Jobs",
                column: "TaskScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_TaskSchedules_TaskScheduleId",
                table: "Jobs",
                column: "TaskScheduleId",
                principalTable: "TaskSchedules",
                principalColumn: "Id");
        }
    }
}
