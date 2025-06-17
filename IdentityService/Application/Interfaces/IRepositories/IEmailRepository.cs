using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces.IRepositories
{
    public interface IEmailRepository
    {
        Task<User> GetUserByEmail(string email);
        Task UpdateConfirmation(Guid userId);
    }
}
