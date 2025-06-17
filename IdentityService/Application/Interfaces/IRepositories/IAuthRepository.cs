using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces.IRepositories
{
    public interface IAuthRepository
    {
        Task Register(User newUser);
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserById(Guid userId);
        Task<bool> ResetPassword(string email,string newPassword);
    }
}
