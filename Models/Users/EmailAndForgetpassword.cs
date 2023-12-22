using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Users
{
    //---------------------------------------------------  Email  -----------------------------------------------------

    public class VerifyEmailRequest
    {
        [Required]
        public string Token { get; set; }
    }



    //---------------------------------------------------  Password  -----------------------------------------------------

    public class ForgotPasswordRequest
    {
        [Required]
        public string Email { get; set; }
    }

    public class ResetPasswordRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Otp { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }


}
