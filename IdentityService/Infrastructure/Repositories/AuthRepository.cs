using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AuthRepository:IAuthRepository
    {
        private readonly IdentityDbContext context;
        private readonly IMapper mapper;
        public AuthRepository(IdentityDbContext _context)
        {
            context = _context;

        }
        public async Task Register(User newUser)
        {

                await context.Users.AddAsync(newUser);
                await context.SaveChangesAsync();

        }

        public async Task<User> GetUserByEmail(string email) {
            return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetUserById(Guid userId) {
            return await context.Users.FirstOrDefaultAsync(user => user.Id == userId);
        }

        public async Task<bool> ResetPassword(string email, string newPassword) {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null) { 
                user.PasswordHash = newPassword;
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }



    }
}
