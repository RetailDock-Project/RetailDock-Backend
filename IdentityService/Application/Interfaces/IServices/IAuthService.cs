using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Common;
using Domain.Entities;

namespace Application.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<ResponseDto<object>> Register(AddUserDto newUser,Guid orgId);
        Task<ResponseDto<string>> Login(LoginDto user);
        Task<User> GetUserById(Guid userId);
        Task<ResponseDto<object>> RequestOtp(string userEmail);
        Task<ResponseDto<object>> VerifyOtp(string email, string otp);

        Task<ResponseDto<object>> ResetPassword(string email, string otp, string newPassword);
    }
}
