namespace STD.Models.Form
{
    public class UserAuditorForm
    {
        public Guid? AuditorID { get; set; }

        public string AuditorCode { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public int Role { get; set; }
    }
}