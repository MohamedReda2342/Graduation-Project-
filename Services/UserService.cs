namespace WebApi.Services;
using MailKit.Net.Smtp;
using MimeKit;
using AutoMapper;
using BCrypt.Net;
using WebApi.Authorization;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models;
using WebApi.Models.Users;
using System.Net.Mail;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using Microsoft.Extensions.Options;

public interface IUserService
{
    AuthenticateResponse Authenticate(AuthenticateRequest model);
    IEnumerable<User> GetAll();
    User GetById(int id);
    void Register(RegisterRequest model);
    void Update(int id, UpdateRequest model);
    void Delete(int id);
    void ForgotPassword(string email);
    void VerifyAndResetPassword(string email, string otp, string newPassword);
    void VerifyEmail(string token);
}

public class UserService : IUserService
{
    private DataContext _context;
    private IJwtUtils _jwtUtils;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration; 
    private readonly AppSettings _appSettings;


// Class Constructor
 public UserService( DataContext context, IJwtUtils jwtUtils, IMapper mapper, IConfiguration configuration ,IOptions<AppSettings> appSettings)
    {
        _context = context;
        _jwtUtils = jwtUtils;
        _mapper = mapper;
        _configuration = configuration;
        _appSettings = appSettings.Value;
    }




    public AuthenticateResponse Authenticate(AuthenticateRequest model)
    {
        var user = _context.Users.SingleOrDefault(x => x.Email == model.Email);

        // validate
        if (user == null || !BCrypt.Verify(model.Password, user.PasswordHash))
            throw new AppException("Email or password is incorrect");
        if (user.IsEmailVerified==false)
            throw new AppException("Email isn't verified");
        user.DeviceToken = model.DeviceToken;
        // authentication successful
        var response = _mapper.Map<AuthenticateResponse>(user);
        _context.Users.Update(user);
        _context.SaveChanges();
        response.Token = _jwtUtils.GenerateToken(user);
        return response;
    }

    public IEnumerable<User> GetAll()
    {
        var users = _context.Users;
        var userResponses = _mapper.Map<IEnumerable<User>>(users);
        return userResponses;
    }


    public User GetById(int id)
    {
        var UserResponses = _mapper.Map<User>(getUser(id));

        return UserResponses;
    }

    public void Register(RegisterRequest model)
    {
        // validate
        if (_context.Users.Any(x => x.Email == model.Email))
            throw new AppException("Email '" + model.Email + "' is already taken");

        // map model to new user object
        var user = _mapper.Map<User>(model);

        // hash password
        user.PasswordHash = BCrypt.HashPassword(model.Password);

        user.VerificationToken = Guid.NewGuid().ToString();
        user.VerificationTokenExpiry = DateTime.UtcNow.AddHours(1000); // Set token expiry time

        // save user
        _context.Users.Add(user);
        _context.SaveChanges();

        // Send verification email
        SendVerificationEmail(user.Email, user.VerificationToken, _appSettings);
    }
    

    public void Update(int id, UpdateRequest model)
    {
        var user = getUser(id);

        // validate
        if (model.Email != user.Email && _context.Users.Any(x => x.Email == model.Email))
            throw new AppException("Email '" + model.Email + "' is already taken");

        if (model.Email != user.Email){
            // Update the user's email
            user.Email = model.Email;

            // Set a new verification token and send a verification email
            user.VerificationToken = Guid.NewGuid().ToString();
            // Set token expiry time
            user.VerificationTokenExpiry = DateTime.UtcNow.AddHours(1000); 

            SendVerificationEmail(user.Email, user.VerificationToken, _appSettings);
            // copy model to user and save
            _mapper.Map(model, user);
            _context.Users.Update(user);
            _context.SaveChanges();
            throw new AppException("User updated successfully , Check your mail for verification");

        }

        // copy model to user and save
        _mapper.Map(model, user);
        _context.Users.Update(user);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var user = getUser(id);
        _context.Users.Remove(user);
        _context.SaveChanges();
    }

    // Helper methods

    private User getUser(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null) throw new KeyNotFoundException("User not found");

        return user;
    }

    //---------------------------------------------------------------------------------------------------------


    private void SendVerificationEmail(string email, string verificationToken , AppSettings appSettings)
    {
        //var emailSettings = _configuration.GetSection("EmailSettings").Get<EmailSettings>();

        var emailSettingsSection = _configuration.GetSection("EmailSettings");
        var emailSettings = emailSettingsSection.Get<EmailSettings>();


        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(emailSettings.FromName, emailSettings.FromEmail));
        message.To.Add(new MailboxAddress("Recipient", email));
        message.Subject = "Account Verification";
          
        var verificationLink = $"{_appSettings.EmailSettings.AppUrl}/users/verify-email?token={verificationToken}";
        var bodyBuilder = new BodyBuilder();
        bodyBuilder.HtmlBody = $"<p>Please click the following link to verify your email: <a href='{verificationLink}'>Verify Email</a></p>";

        message.Body = bodyBuilder.ToMessageBody();

        using (var client = new SmtpClient())
        {
            client.Connect(emailSettings.SmtpServer, emailSettings.SmtpPort, true);

            client.Authenticate(emailSettings.SmtpUsername, emailSettings.SmtpPassword);

            client.Send(message);
            client.Disconnect(true);
        }
    }


    public void VerifyEmail(string token)
    {
        var user = _context.Users.SingleOrDefault(x => x.VerificationToken == token);

        // validate
        if (user == null)
            throw new AppException("Invalid verification token");

        if (user.IsEmailVerified)
            throw new AppException("Email already verified");

        if (user.VerificationTokenExpiry.HasValue && user.VerificationTokenExpiry < DateTime.UtcNow)
            throw new AppException("Verification token has expired");

        // Mark the email as verified
        user.IsEmailVerified = true;
        user.VerificationToken = null;
        user.VerificationTokenExpiry = null;

        _context.SaveChanges();
    }




    //---------------------------------------------------  Password  -----------------------------------------------------

    public void ForgotPassword(string email)
    {
        var user = _context.Users.SingleOrDefault(x => x.Email == email);

        // validate
        if (user == null)
            throw new AppException("User not found with the provided email");

        // Generate and store OTP
        user.ResetToken = GenerateOTP();
        // Set OTP expiry time 15 Minutes
        user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(15); 

        _context.SaveChanges();

        // Send forgot password email with OTP
        SendForgotPasswordEmail(user.Email, user.ResetToken);
    }




    private void SendForgotPasswordEmail(string email, string resetToken)
    {
        var emailSettings = _configuration.GetSection("EmailSettings").Get<EmailSettings>();

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(emailSettings.FromName, emailSettings.FromEmail));
        message.To.Add(new MailboxAddress("Recipient", email));
        message.Subject = "Forgot Password";

        var bodyBuilder = new BodyBuilder();
        bodyBuilder.HtmlBody = $"<h2>Please enter this OTP in the application {resetToken}</h2>";

        message.Body = bodyBuilder.ToMessageBody();

        using (var client = new SmtpClient())
        {
            client.Connect(emailSettings.SmtpServer, emailSettings.SmtpPort, true);

            client.Authenticate(emailSettings.SmtpUsername, emailSettings.SmtpPassword);

            client.Send(message);
            client.Disconnect(true);
        }
    }


    public void VerifyAndResetPassword(string email, string otp, string newPassword)
    {
        var user = _context.Users.SingleOrDefault(x => x.Email == email);

        // validate
        if (user == null)
            throw new AppException("User not found with the provided email");

        if (user.ResetToken != otp || user.ResetTokenExpiry == null || user.ResetTokenExpiry < DateTime.UtcNow)
            throw new AppException("Invalid or expired OTP");

        // Reset password
        user.PasswordHash = BCrypt.HashPassword(newPassword);
        user.ResetToken = null;
        user.ResetTokenExpiry = null;

        _context.SaveChanges();
    }

    private string GenerateOTP()
    {
        var otp = new Random().Next(100000, 999999).ToString();
        return otp;
    }
 
}



