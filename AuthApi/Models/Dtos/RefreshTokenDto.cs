namespace AuthApi.Models.Dtos
{
    public class RefreshTokenDto
    {
        public string Token { get; set; }
        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
    }
}
