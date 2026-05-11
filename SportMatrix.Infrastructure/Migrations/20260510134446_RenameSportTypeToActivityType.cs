using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportMatrix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameSportTypeToActivityType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SportType",
                table: "PlannedActivities");

            migrationBuilder.DropColumn(
                name: "SportType",
                table: "Activities");

            migrationBuilder.AddColumn<int>(
                name: "ActivityType",
                table: "PlannedActivities",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ActivityType",
                table: "Activities",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Activities",
                keyColumn: "Id",
                keyValue: 1,
                column: "ActivityType",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Activities",
                keyColumn: "Id",
                keyValue: 2,
                column: "ActivityType",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Activities",
                keyColumn: "Id",
                keyValue: 3,
                column: "ActivityType",
                value: 3);

            migrationBuilder.UpdateData(
                table: "PlannedActivities",
                keyColumn: "Id",
                keyValue: 1,
                column: "ActivityType",
                value: 1);

            migrationBuilder.UpdateData(
                table: "PlannedActivities",
                keyColumn: "Id",
                keyValue: 2,
                column: "ActivityType",
                value: 1);

            migrationBuilder.UpdateData(
                table: "PlannedActivities",
                keyColumn: "Id",
                keyValue: 3,
                column: "ActivityType",
                value: 7);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivityType",
                table: "PlannedActivities");

            migrationBuilder.DropColumn(
                name: "ActivityType",
                table: "Activities");

            migrationBuilder.AddColumn<string>(
                name: "SportType",
                table: "PlannedActivities",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SportType",
                table: "Activities",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Activities",
                keyColumn: "Id",
                keyValue: 1,
                column: "SportType",
                value: "Run");

            migrationBuilder.UpdateData(
                table: "Activities",
                keyColumn: "Id",
                keyValue: 2,
                column: "SportType",
                value: "Ride");

            migrationBuilder.UpdateData(
                table: "Activities",
                keyColumn: "Id",
                keyValue: 3,
                column: "SportType",
                value: "Swim");

            migrationBuilder.UpdateData(
                table: "PlannedActivities",
                keyColumn: "Id",
                keyValue: 1,
                column: "SportType",
                value: "Run");

            migrationBuilder.UpdateData(
                table: "PlannedActivities",
                keyColumn: "Id",
                keyValue: 2,
                column: "SportType",
                value: "Run");

            migrationBuilder.UpdateData(
                table: "PlannedActivities",
                keyColumn: "Id",
                keyValue: 3,
                column: "SportType",
                value: "Brick");
        }
    }
}
