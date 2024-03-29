namespace WebApi.Models.Users;

public class AuthenticateResponse
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public int? PhoneNumber { get; set; }    
    public string Email { get; set; }
    public string Token { get; set; }
}