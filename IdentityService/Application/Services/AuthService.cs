using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using AutoMapper;
using Common;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public class AuthService:IAuthService
    {
        private readonly IAuthRepository userRepository;
        private readonly IEmailService emailSerivce;
        private readonly IMapper mapper;
        private readonly IJwtService jwtService;
        private readonly IOtpService otpService;
        private readonly IHttpContextAccessor httpContextAccessor;


        public AuthService(IAuthRepository _userRepository,IMapper _mapper, IJwtService _jwtService, IEmailService _emailSerivce, IOtpService _otpService, IHttpContextAccessor _httpContextAccessor) { 
            mapper = _mapper;
            userRepository = _userRepository;
            jwtService = _jwtService;
            emailSerivce=_emailSerivce;
            otpService = _otpService;
            httpContextAccessor= _httpContextAccessor;
        }
        public async Task<ResponseDto<object>> Register(AddUserDto newUser,Guid orgId)
        {
            var emailAlreadyExist = await userRepository.GetUserByEmail(newUser.Email);
            if (emailAlreadyExist != null) {
                return new ResponseDto<object> { StatusCode = 409, Message = "Email already exist" };

            }
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
            var user = mapper.Map<User>(newUser);
            if (orgId != null || orgId!=Guid.Empty) {
                user.OrganisationId = orgId;
            }
            user.PasswordHash=hashedPassword;
            user.EmailVerificationToken=Guid.NewGuid().ToString();
            await userRepository.Register(user);
            var verificationLink = $"https://localhost:7117/api/Email/confirm?email={user.Email}&token={user.EmailVerificationToken}";

            string body = $@"
                            <html>
                                <head>
                                    <style>
                                        .btn {{
                                            display: inline-block;
                                            padding: 10px 20px;
                                            background-color: #4CAF50;
                                            color: white;
                                            text-decoration: none;
                                            border-radius: 5px;
                                        }}
                                        .container {{
                                            font-family: Arial, sans-serif;
                                            padding: 20px;
                                            background-color: #f9f9f9;
                                            border: 1px solid #ddd;
                                            border-radius: 10px;
                                            max-width: 600px;
                                            margin: auto;
                                        }}
                                        .title {{
                                            color: #333;
                                        }}
                                    </style>
                                </head>
                                <body>
                                    <div class='container'>
                                        <h2 class='title'>Verify Your Email</h2>
                                        <p>Hi,</p>
                                        <p>Thank you for registering. Please click the button below to verify your email address:</p>
                                        <a href='{verificationLink}' style='background:#4CAF50;color:white;padding:10px 20px;margin:10px 10px;border-radius:5px;text-decoration:none;'>Verify Email</a>

                                        <p>If you did not request this, please ignore this email.</p>
                                        <br />
                                        <p>Best regards,<br/>Your Company</p>
                                    </div>
                                </body>
                            </html>";
            await emailSerivce.SendEmailAsync(newUser.Email, "Verify your email",body);
            return new ResponseDto<object> { StatusCode = 200, Message = "Registration successful. Verify Email and Sign In" };

        }

        public async Task<ResponseDto<string>> Login(LoginDto loginData)
        {
            var user=await userRepository.GetUserByEmail(loginData.Email);
            bool isValid = BCrypt.Net.BCrypt.Verify(loginData.Password, user.PasswordHash);

            if (user == null || !isValid) {
                return new ResponseDto<string> { StatusCode = 404, Message ="Invalid Credentials" };

            }
            var token = await jwtService.GenerateToken(user);
            var context = httpContextAccessor.HttpContext;

            if (context != null)
            {
                context.Response.Cookies.Append("accessToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.Now.AddDays(7)
                });
            }
            return new ResponseDto<string> { StatusCode = 200,Data=token, Message = "login successful" };
        }

        public async Task<User> GetUserById(Guid userId) { 
            var user= await userRepository.GetUserById(userId);
            return user;
        }

        public async Task<ResponseDto<object>> RequestOtp(string userEmail) {
            var response=await otpService.GenerateOtpAndMail(userEmail);
            return new ResponseDto<object> { StatusCode = response.StatusCode, Message = response.Message };
        }
        public async Task<ResponseDto<object>> VerifyOtp(string email,string otp) { 
            var response=await otpService.VerifyOtp(email, otp);
            return new ResponseDto<object> {StatusCode = response.StatusCode,Message = response.Message};
        }

        public async Task<ResponseDto<object>> ResetPassword(string email,string otp, string newPassword) {
            var isVerified = await otpService.VerifyOnReset(email, otp);
            if (!isVerified) {
                return new ResponseDto<object> { StatusCode = 400, Message = "Invalid otp verification" };

            }
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
            var isSuccess=await userRepository.ResetPassword(email, hashedPassword);
            if (!isSuccess) {
                return new ResponseDto<object> { StatusCode = 400, Message = "Password reset failed" };

            }
            return new ResponseDto<object> { StatusCode = 200, Message = "Password reset successfully" };

        }

    }
}
