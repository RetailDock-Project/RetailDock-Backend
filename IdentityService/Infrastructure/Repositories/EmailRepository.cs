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
    public class EmailRepository:IEmailRepository
    {
        private readonly IdentityDbContext context;
        public EmailRepository(IdentityDbContext _context)
        {

            context = _context;
        }
        public async Task<User> GetUserByEmail(string email)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task UpdateConfirmation(Guid userId)
        {
            var user=await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            user.IsEmailConfirmed=true;
            user.EmailVerificationToken=null;
            await context.SaveChangesAsync();
        }

        
    }
}
