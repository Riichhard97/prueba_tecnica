namespace Nexu.Shared
{
    public static class AppConstants
    {
        //public const int ShortDescLength = 2;
        public const int NameLength = 50;
        public const int EmailLength = 256;
        public const int StandardValueLength = 100;
        public const int StandardLocationLength = 512;
        public const int CommentsLength = 150;

        public const int PostalCodeLength = 10;
        public const int PhoneNumberLength = 20;
        public const int CountryCodeLength = 3;
        public const int LongValueLength = 1024;
        public const int MaxLength = 4000;
        public const int ProblemDescriptionLength = 500;

        public const int CurrencyLength = 3;
        public const int CountryIso2Length = 2;

        public const int LanguageIso2Length = 2;

        public const int MinValueLength = 6;

        public const int MinPasswordLength = 6;
        public const int MaxPasswordLength = 50;

        public const int KeywordMinLength = 2;
        public const int KeywordMaxLength = 12;

        public const int SmsMaxLength = 160;
        public const int MmsMaxLength = 1600;

        public const int EmailCodeLength = 40;

        public const string PasswordComplexity = @"^(?:(?=.*[a-z])(?:(?=.*[A-Z])(?=.*\d))).{6,}$";

        public static class SettingsPassword
        {
            public const string ForgotPasswordName = "change_password";
            public const string NewPasswordName = "new_password";
            public const int ForgetPasswordTimeExpirated = 10;
            public const int NewPasswordTimeExpirated = 1441;
        }

        public static class TwoFactorType
        {
            public const string Email = "Email";
        }
    }
}
