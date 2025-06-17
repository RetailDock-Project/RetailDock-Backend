using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IRepositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class OtpRepository: IOtpRepository
    {
        private readonly IdentityDbContext context;
        public OtpRepository(IdentityDbContext _context)
        {
            context = _context;

        }
        public async Task<User> GetUserByEmail(string email) {
            return await context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}
