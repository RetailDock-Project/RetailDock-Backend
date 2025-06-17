using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Application.Interfaces.IServices
{
    public interface IOtpService
    {
        Task<ResponseDto<object>> GenerateOtpAndMail(string email);
        Task<ResponseDto<object>> VerifyOtp(string email, string otp);
        Task<bool> VerifyOnReset(string email, string otp);
    }
}
