using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateSqlServerIntGender : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FoodItems",
                columns: table => new
                {
                    FoodId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CaloriesPerServing = table.Column<double>(type: "float", nullable: false),
                    ServingSizeDescription = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProteinGrams = table.Column<double>(type: "float", nullable: true),
                    CarbGrams = table.Column<double>(type: "float", nullable: true),
                    FatGrams = table.Column<double>(type: "float", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImagePublicId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodItems", x => x.FoodId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: true),
                    HeightCm = table.Column<double>(type: "float", nullable: true),
                    WeightKg = table.Column<double>(type: "float", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsPremium = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EmailVerified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    GoogleId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutPlans",
                columns: table => new
                {
                    PlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EstimatedDurationMinutes = table.Column<int>(type: "INTEGER", nullable: true),
                    EstimatedCaloriesBurned = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutPlans", x => x.PlanId);
                });

            migrationBuilder.CreateTable(
                name: "Workouts",
                columns: table => new
                {
                    WorkoutId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TargetMuscleGroup = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DefaultReps = table.Column<int>(type: "INTEGER", nullable: true),
                    DefaultDurationSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    VideoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workouts", x => x.WorkoutId);
                });

            migrationBuilder.CreateTable(
                name: "DailyActivity",
                columns: table => new
                {
                    ActivityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    StepsCount = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    WaterIntakeMl = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    SleepDurationMinutes = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    ActiveCaloriesBurned = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    RestingCaloriesBurned = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyActivity", x => x.ActivityId);
                    table.ForeignKey(
                        name: "FK_DailyActivity_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Goals",
                columns: table => new
                {
                    GoalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    GoalType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TargetValue = table.Column<float>(type: "REAL", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goals", x => x.GoalId);
                    table.ForeignKey(
                        name: "FK_Goals_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MealLog",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FoodId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MealType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    TotalCalories = table.Column<double>(type: "float", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealLog", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_MealLog_FoodItems_FoodId",
                        column: x => x.FoodId,
                        principalTable: "FoodItems",
                        principalColumn: "FoodId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MealLog_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OrderInfo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VnpayTransactionNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VnpayResponseCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    VnpayBankCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledMeals",
                columns: table => new
                {
                    ScheduleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    MealType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PlannedFoodId = table.Column<int>(type: "int", nullable: true),
                    PlannedDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Planned")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledMeals", x => x.ScheduleId);
                    table.ForeignKey(
                        name: "FK_ScheduledMeals_FoodItems_PlannedFoodId",
                        column: x => x.PlannedFoodId,
                        principalTable: "FoodItems",
                        principalColumn: "FoodId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ScheduledMeals_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SleepLog",
                columns: table => new
                {
                    SleepLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    QualityRating = table.Column<int>(type: "INTEGER", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SleepLog", x => x.SleepLogId);
                    table.ForeignKey(
                        name: "FK_SleepLog_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutPlanItems",
                columns: table => new
                {
                    PlanItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanId = table.Column<int>(type: "int", nullable: false),
                    WorkoutId = table.Column<int>(type: "int", nullable: false),
                    ItemOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    Sets = table.Column<int>(type: "INTEGER", nullable: true),
                    Reps = table.Column<int>(type: "INTEGER", nullable: true),
                    DurationSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    RestBetweenSetsSeconds = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutPlanItems", x => x.PlanItemId);
                    table.ForeignKey(
                        name: "FK_WorkoutPlanItems_WorkoutPlans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "WorkoutPlans",
                        principalColumn: "PlanId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkoutPlanItems_Workouts_WorkoutId",
                        column: x => x.WorkoutId,
                        principalTable: "Workouts",
                        principalColumn: "WorkoutId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutSessions",
                columns: table => new
                {
                    SessionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PlanId = table.Column<int>(type: "int", nullable: true),
                    WorkoutId = table.Column<int>(type: "int", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurationSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    CaloriesBurned = table.Column<int>(type: "INTEGER", nullable: true),
                    AvgHeartRate = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxHeartRate = table.Column<int>(type: "INTEGER", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutSessions", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_WorkoutSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkoutSessions_WorkoutPlans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "WorkoutPlans",
                        principalColumn: "PlanId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_WorkoutSessions_Workouts_WorkoutId",
                        column: x => x.WorkoutId,
                        principalTable: "Workouts",
                        principalColumn: "WorkoutId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyActivity_UserId_Date",
                table: "DailyActivity",
                columns: new[] { "UserId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FoodItems_Name",
                table: "FoodItems",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Goals_UserId",
                table: "Goals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MealLog_FoodId",
                table: "MealLog",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_MealLog_UserId",
                table: "MealLog",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_OrderId",
                table: "PaymentTransactions",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_UserId",
                table: "PaymentTransactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledMeals_PlannedFoodId",
                table: "ScheduledMeals",
                column: "PlannedFoodId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledMeals_UserId",
                table: "ScheduledMeals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SleepLog_UserId",
                table: "SleepLog",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_GoogleId",
                table: "Users",
                column: "GoogleId",
                unique: true,
                filter: "[GoogleId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutPlanItems_PlanId",
                table: "WorkoutPlanItems",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutPlanItems_WorkoutId",
                table: "WorkoutPlanItems",
                column: "WorkoutId");

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_Name",
                table: "Workouts",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessions_PlanId",
                table: "WorkoutSessions",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessions_UserId",
                table: "WorkoutSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessions_WorkoutId",
                table: "WorkoutSessions",
                column: "WorkoutId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyActivity");

            migrationBuilder.DropTable(
                name: "Goals");

            migrationBuilder.DropTable(
                name: "MealLog");

            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "ScheduledMeals");

            migrationBuilder.DropTable(
                name: "SleepLog");

            migrationBuilder.DropTable(
                name: "WorkoutPlanItems");

            migrationBuilder.DropTable(
                name: "WorkoutSessions");

            migrationBuilder.DropTable(
                name: "FoodItems");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "WorkoutPlans");

            migrationBuilder.DropTable(
                name: "Workouts");
        }
    }
}
