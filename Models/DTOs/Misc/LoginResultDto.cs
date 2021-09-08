namespace Models.DTOs.Misc
{
    public class LoginResultDto
    {
        public long Id { get; set; }
        public string Token { get; set; }

        public LoginResultDto(long id, string token)
        {
            Id = id;
            Token = token;
        }
    }
}