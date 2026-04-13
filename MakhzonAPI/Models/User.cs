namespace MakhzonAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } // سنستخدمها للتحقق من الدخول
        public string Role { get; set; } // Admin أو Worker
        public string PlayerId { get; set; }
    }
}
