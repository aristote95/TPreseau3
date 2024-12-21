namespace TPreseau3.Models
{
    public class SecuritySetting
    {
        public int SettingId { get; set; }
        public int MaxAttempts { get; set; }
        public int PasswordMinLength { get; set; }
        public string PasswordComplexity { get; set; } = string.Empty;
    }
}