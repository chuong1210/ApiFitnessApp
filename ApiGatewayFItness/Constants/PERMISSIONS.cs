namespace ApiGatewayFitness.Constants
{
    public static class PERMISSIONS
    {
        public const string ViewProfile = "Permissions.Profile.View";
        public const string EditProfile = "Permissions.Profile.Edit";
        public const string LogWorkout = "Permissions.Workout.Log"; // Có thể user thường cũng log được?

        // Quyền chỉ dành cho Premium Users
        public const string ViewPremiumDashboard = "Permissions.Dashboard.ViewPremium";
        public const string AccessPremiumPlans = "Permissions.WorkoutPlans.AccessPremium";
        public const string GenerateReports = "Permissions.Reports.Generate";
        public const string AdvancedNutritionTracking = "Permissions.Nutrition.TrackAdvanced";

        // Quyền chỉ dành cho Standard Users (ít phổ biến hơn, nhưng có thể)
        public const string ViewStandardDashboard = "Permissions.Dashboard.ViewStandard";

        // Có thể gom nhóm nếu cần, ví dụ:
        public static class Profile
        {
            public const string View = "Permissions.Profile.View";
            public const string Edit = "Permissions.Profile.Edit";
        }
    }
}
