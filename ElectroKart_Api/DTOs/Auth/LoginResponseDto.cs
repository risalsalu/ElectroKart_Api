namespace ElectroKart_Api.DTOs.Auth
{
    public class LoginResponseDto
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public bool IsBlocked { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
