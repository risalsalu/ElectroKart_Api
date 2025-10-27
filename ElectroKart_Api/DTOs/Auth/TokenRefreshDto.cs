namespace ElectroKart_Api.DTOs.Auth
{
    public class TokenRefreshDto
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
