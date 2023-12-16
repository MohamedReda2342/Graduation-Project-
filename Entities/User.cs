namespace WebApi.Entities;

using System.Text.Json.Serialization;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public int? PhoneNumber { get; set; }
    public string Email { get; set; }
    public List<Patient> Patients { get; set; } = new List<Patient>();



    [JsonIgnore]
    public string PasswordHash { get; set; }

}




