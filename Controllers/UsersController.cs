namespace WebApi.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.Authorization;
using WebApi.Helpers;
using WebApi.Models.Users;
using WebApi.Services;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private IUserService _userService;
    private IMapper _mapper;
    private readonly AppSettings _appSettings;

    public UsersController( IUserService userService, IMapper mapper, IOptions<AppSettings> appSettings)
    {
        _userService = userService;
        _mapper = mapper;
        _appSettings = appSettings.Value;
    }



    [AllowAnonymous]
    [HttpPost("register")]
    public IActionResult Register([FromForm] RegisterRequest model)
    {
        _userService.Register(model);
        return Ok(new { message = "Check your mail for verification" });
    }



    [AllowAnonymous]
    [HttpPost("authenticate")]
    public IActionResult Authenticate([FromForm] AuthenticateRequest model)
    {
        var response = _userService.Authenticate(model);
        return Ok(response);
    }




    [HttpGet("Get-all")]
    public IActionResult GetAll()
    {
        var users = _userService.GetAll();
        return Ok(users);
    }

    [HttpGet("get-by-id")]
    public IActionResult GetById([FromQuery] int id)
    {
        var user = _userService.GetById(id);
        return Ok(user);
    }

    [HttpPut("edit")]
    public IActionResult Update([FromForm] int id, [FromForm] UpdateRequest model)
    {
        _userService.Update(id, model);
        return Ok(new { message = "User updated successfully" });
    }

    [HttpPut("delete")]
    public IActionResult Delete([FromForm] int id)
    {
        _userService.Delete(id);
        return Ok(new { message = "User deleted successfully" });
    }



    //----------------------------------------------------------------------------------------

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public IActionResult ForgotPassword([FromForm] ForgotPasswordRequest model)
    {
        try
        {
            _userService.ForgotPassword(model.Email);
            return Ok(new { message = "OTP sent successfully" });
        }
        catch (AppException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    [AllowAnonymous]
    [HttpPost("reset-password")]
    public IActionResult ResetPassword([FromForm] ResetPasswordRequest model)
    {
        try
        {
            _userService.VerifyAndResetPassword(model.Email, model.Otp, model.NewPassword);
            return Ok(new { message = "Password reset successfully" });
        }
        catch (AppException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }



    [AllowAnonymous]
    [HttpGet("verify-email")]
    public IActionResult VerifyEmail([FromQuery] VerifyEmailRequest model)
    {
        try
        {
            _userService.VerifyEmail(model.Token);
            return Ok(new { message = "Email verified successfully" });
        }
        catch (AppException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
                                                           
    }

    



}