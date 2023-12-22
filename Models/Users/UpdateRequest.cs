namespace WebApi.Models.Users;

public class UpdateRequest
{
    public string FullName { get; set; }
    public int? PhoneNumber { get; set; }
    public string Email { get; set; }
}