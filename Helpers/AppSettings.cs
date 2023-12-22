namespace WebApi.Helpers;

public class AppSettings
{
    public string Secret { get; set; }

    public EmailSettings EmailSettings { get; set; } = new EmailSettings();
    public string AppUrl { get; set; }

}

public class EmailSettings
{
    public string SmtpServer { get; set; } = "smtp.example.com"; // Provide default value
    public int SmtpPort { get; set; } = 587; // Provide default value
    public string SmtpUsername { get; set; } = "reda23425@gmail.com"; // Provide default value
    public string SmtpPassword { get; set; } = "dxxr esaz tjbm xvzq"; // Provide default value
    public string FromEmail { get; set; } = "reda23425@gmail.com"; // Provide default value
    public string FromName { get; set; } = "Elder People Band"; // Provide default value
    public string AppUrl { get; set; } = "testgd.azurewebsites.net"; // Provide default value
}
//public class EmailSettings
//{
//    public string SmtpServer { get; set; } = "sandbox.smtp.mailtrap.io"; // Provide default value
//    public int SmtpPort { get; set; } = 587; // Provide default value
//    public string SmtpUsername { get; set; } = "f49e6cc51cccab"; // Provide default value
//    public string SmtpPassword { get; set; } = "2204fa760913bc"; // Provide default value
//    public string FromEmail { get; set; } = "reda23425@gmail.com"; // Provide default value
//    public string FromName { get; set; } = "Elder People Band"; // Provide default value
//    public string AppUrl { get; set; } = "testgd.azurewebsites.net"; // Provide default value
//}



