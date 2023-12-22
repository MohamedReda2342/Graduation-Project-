namespace WebApi.Entities;

using System.Text.Json.Serialization;

public class User
{

    public int Id { get; set; }
    public string FullName { get; set; }
    public int? PhoneNumber { get; set; }
    public string Email { get; set; }

    //Password
    public string ResetToken { get; set; }
    public DateTime? ResetTokenExpiry { get; set; }

    //Email
    public bool IsEmailVerified { get; set; }
    public string VerificationToken { get; set; }
    public DateTime? VerificationTokenExpiry { get; set; }

    // Patient
    public List<Patient> Patients { get; set; } = new List<Patient>();

    [JsonIgnore]
    public string PasswordHash { get; set; }




}





