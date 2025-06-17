using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces.IServices
{
    public interface IJwtService
    {
        Task<string> GenerateToken(User user);

    }
}
