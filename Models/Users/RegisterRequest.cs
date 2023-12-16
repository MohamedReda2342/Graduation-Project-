namespace WebApi.Models.Users;

using System.ComponentModel.DataAnnotations;

public class RegisterRequest
{
    [Required]
    public string FullName { get; set; }

    [Required]
    public int? PhoneNumber { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}