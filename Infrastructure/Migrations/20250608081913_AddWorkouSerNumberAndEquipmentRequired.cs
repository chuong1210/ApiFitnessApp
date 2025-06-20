using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkouSerNumberAndEquipmentRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EquipmentNeeded",
                table: "Workouts");

            migrationBuilder.AddColumn<string>(
                name: "RequiredEquipment",
                table: "Workouts",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Difficulty",
                table: "WorkoutPlans",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SetNumber",
                table: "WorkoutPlanItems",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiredEquipment",
                table: "Workouts");

            migrationBuilder.DropColumn(
                name: "Difficulty",
                table: "WorkoutPlans");

            migrationBuilder.DropColumn(
                name: "SetNumber",
                table: "WorkoutPlanItems");

            migrationBuilder.AddColumn<string>(
                name: "EquipmentNeeded",
                table: "Workouts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
