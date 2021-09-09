namespace Models.DTOs.Login
{
    public class LoginResultDto
    {
        public long Id { get; set; }
        public string Token { get; set; }
        public bool HasSubscription { get; set; }

        public LoginResultDto(long id, string token, bool hasSubscription)
        {
            Id = id;
            Token = token;
            HasSubscription = hasSubscription;
        }
    }
}