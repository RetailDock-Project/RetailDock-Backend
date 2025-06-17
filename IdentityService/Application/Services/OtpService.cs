using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Common;
using StackExchange.Redis;
namespace Application.Services
{
    public class OtpService:IOtpService
    {
        private readonly IDatabase _redis;
        private readonly IOtpRepository otpRepository;
        private readonly IEmailService emailService;


        public OtpService(IConnectionMultiplexer redis,IOtpRepository _otpRepository, IEmailService _emailService)
        {
            _redis = redis.GetDatabase();
            otpRepository = _otpRepository;
            emailService = _emailService;
        }

        public async Task<ResponseDto<object>> GenerateOtpAndMail(string email)
        {
            var user=await otpRepository.GetUserByEmail(email);
            if (user == null) {
                return new ResponseDto<object> { StatusCode = 400,Message="Invalid Email" };
            }
            var otp = new Random().Next(100000, 999999).ToString();
            await _redis.StringSetAsync($"otp:{email}", otp, TimeSpan.FromMinutes(5));
            string body = $@"
    <html>
    <head>
        <style>
            @media only screen and (max-width: 600px) {{
                .content {{
                    padding: 10px !important;
                }}
            }}
        </style>
    </head>
    <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 30px;'>
        <div style='max-width: 600px; margin: auto; background-color: white; padding: 20px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
            <h2 style='color: #2c3e50;'>Hi {user.Name ?? "User"},</h2>
            <p style='font-size: 16px; color: #555;'>We received a request to reset your password. Use the OTP below to proceed:</p>
            <div style='text-align: center; margin: 30px 0;'>
                <p style='font-size: 30px; color: #e74c3c; letter-spacing: 5px;'><strong>{otp}</strong></p>
            </div>
            <p style='font-size: 14px; color: #888;'>This OTP is valid for <strong>5 minutes</strong>. Please do not share it with anyone.</p>
            <p style='font-size: 14px; color: #888;'>If you did not request a password reset, you can safely ignore this email.</p>
            <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;' />
            <p style='font-size: 12px; color: #aaa; text-align: center;'>Need help? Contact us at <a href='mailto:support@retaildock.com'>support@retaildock.com</a></p>
        </div>
    </body>
    </html>";
            await emailService.SendEmailAsync(email, "Reset Your Password - OTP", body);
            return new ResponseDto<object> { StatusCode = 200, Message = "OTP generated and sent to email" };


        }

        public async Task<ResponseDto<object>> VerifyOtp(string email, string otp)
        {
            var storedOtp = await _redis.StringGetAsync($"otp:{email}");
            if (storedOtp == otp)
            {
                //await _redis.KeyDeleteAsync($"otp:{email}");
                return new ResponseDto<object> { StatusCode = 200, Message = "OTP verification successful. Reset password" };
            }
            return new ResponseDto<object> { StatusCode = 400, Message = "Invalid Credentials" };
        }

        public async Task<bool> VerifyOnReset(string email, string otp)
        {
            var storedOtp = await _redis.StringGetAsync($"otp:{email}");
            if (storedOtp == otp)
            {
                await _redis.KeyDeleteAsync($"otp:{email}");
                return true;
            }
            return false;
        }


    }
}
