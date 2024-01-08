namespace WebApi.Entities;

using System.Text.Json.Serialization;

public class User
{

    public int Id { get; set; }
    public string FullName { get; set; }
    public int? PhoneNumber { get; set; }
    public string Email { get; set; }

    //Password
    [JsonIgnore]
    public string ResetToken { get; set; }
    [JsonIgnore]
    public DateTime? ResetTokenExpiry { get; set; }

    //Email
    [JsonIgnore]
    public bool IsEmailVerified { get; set; }
    [JsonIgnore]
    public string VerificationToken { get; set; }
    [JsonIgnore]
    public DateTime? VerificationTokenExpiry { get; set; }

    // Patient
    [JsonIgnore]
    public List<Patient> Patients { get; set; } = new List<Patient>();

    [JsonIgnore]
    public string PasswordHash { get; set; } 
    [JsonIgnore]
    public string DeviceToken { get; set; }





}





