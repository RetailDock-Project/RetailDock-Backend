using System.IdentityModel.Tokens.Jwt;
using Application.DTOs;
using Application.Interfaces.IServices;
using Application.Services;
using Domain.Entities;
using IdentityService.Controllers.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace IdentityService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAuthService userService;
        public AuthController(IAuthService _userService)
        {
            userService = _userService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(AddUserDto newUser)
        {
            var response = await userService.Register(newUser,OrgId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto user)
        {
            var response = await userService.Login(user);
            return StatusCode(response.StatusCode, response);
        }



        [HttpGet("me")]
        public async Task<IActionResult> MyDetails()
        {

            Console.WriteLine("this is userid"+UserId);
            var user = await userService.GetUserById(UserId);
            if (user == null)
                return NotFound("User not found.");

            foreach (var r in Roles)
            {
                Console.WriteLine($"Role: {r}");

            }

            var result = new
            {
                user.Id,
                user.Name,
                user.Email,
                OrganisationId = OrgId,
                roles = Roles,
                permissions = Permissions
            };

            return Ok(result);
        }

        [HttpPost("request-otp")]
        public async Task<IActionResult> RequestOtp(ForgotPasswordRequest userEmail)
        {
            var response = await userService.RequestOtp(userEmail.Email);
            return StatusCode(response.StatusCode, response);
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest resetPassword)
        {
            var response = await userService.ResetPassword(resetPassword.Email, resetPassword.Otp,resetPassword.NewPassword);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpRequest verifyOtp)
        {
            var response = await userService.VerifyOtp(verifyOtp.Email, verifyOtp.Otp);
            return StatusCode(response.StatusCode, response);
        }



    }
}
