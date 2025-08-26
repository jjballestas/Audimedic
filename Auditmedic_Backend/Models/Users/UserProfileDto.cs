namespace Audimedic_Backend.Models.Users
{
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Email { get; set; } = "";
        public string Rol { get; set; } = "";
    }

    public class UserUpdateDto
    {
        public string? Nombre { get; set; }
    }

    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = "";
        public string NewPassword { get; set; } = "";
        public string ConfirmPassword { get; set; } = "";
    }
}
