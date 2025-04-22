namespace FitnessApp.Constants
{
    public static class CONSTANT_CLAIM_TYPES
    {
        public const string Uid = "uid";

        public const string UserName = "userName";

        public const string Customer = "customer";

        public const string Type = "type";

        public const string Faculty = "faculty";

        public const string FacultyId = "facultyId";

        public const string Permission = "permission";
    }

    public static class CLAIMS_VALUES
    {
        public static string TYPE_ADMIN = "ADMIN";//User.UserType.Admin.ToString();

        public static string TYPE_SUPPER_ADMIN ="USER";


    }


    public static class CONSTANT_PERMESSION_DEFAULT
    {
        public static List<string> PERMISSIONS = new List<string>
        {
            "order.view",
            "order.change-status",
            "food.view",
            "category.view",
            "coupon.view",
            "payment.view",
        };

        public static List<string> PERMISSIONS_NO_LOGIN = new List<string>
        {
            "food.view",
            "category.view",
        };
    }
}
