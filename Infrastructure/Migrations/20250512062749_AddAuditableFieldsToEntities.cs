using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditableFieldsToEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "WorkoutSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "WorkoutSessions",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "WorkoutSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "WorkoutSessions",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Workouts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Workouts",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Workouts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Workouts",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "WorkoutPlans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "WorkoutPlans",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "WorkoutPlans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "WorkoutPlans",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "WorkoutPlanItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "WorkoutPlanItems",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "WorkoutPlanItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "WorkoutPlanItems",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "SleepLog",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "SleepLog",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "SleepLog",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "SleepLog",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ScheduledMeals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ScheduledMeals",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ScheduledMeals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ScheduledMeals",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "PaymentTransactions",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "PaymentTransactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "PaymentTransactions",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "MealLog",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "MealLog",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "MealLog",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "MealLog",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Goals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Goals",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Goals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Goals",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "FoodItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "FoodItems",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "FoodItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "FoodItems",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "DailyActivity",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "DailyActivity",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "DailyActivity",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "DailyActivity",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "WorkoutSessions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "WorkoutSessions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "WorkoutSessions");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "WorkoutSessions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Workouts");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Workouts");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Workouts");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Workouts");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "WorkoutPlans");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "WorkoutPlans");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "WorkoutPlans");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "WorkoutPlans");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "WorkoutPlanItems");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "WorkoutPlanItems");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "WorkoutPlanItems");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "WorkoutPlanItems");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "SleepLog");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "SleepLog");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "SleepLog");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "SleepLog");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ScheduledMeals");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ScheduledMeals");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ScheduledMeals");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ScheduledMeals");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "MealLog");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "MealLog");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "MealLog");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "MealLog");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "FoodItems");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "FoodItems");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "FoodItems");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "FoodItems");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DailyActivity");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "DailyActivity");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "DailyActivity");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "DailyActivity");
        }
    }
}
