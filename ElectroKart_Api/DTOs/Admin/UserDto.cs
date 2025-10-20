namespace ElectroKart_Api.DTOs.Admin
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public bool IsBlocked { get; set; } = false;
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
