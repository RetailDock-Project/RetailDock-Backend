using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IServices;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly IdentityDbContext context;

        public JwtService(IConfiguration configuration, IdentityDbContext _context)
        {
            _configuration = configuration;
            context = _context;
        }

        public async Task<string> GenerateToken(User user)
        {
                    var roles = await context.UserOrganizationRoles.Include(uor=>uor.OrganizationRole).ThenInclude(or=>or.Role)
            .Where(uor => uor.UserId == user.Id && uor.OrganizationRole.OrganizationId == user.OrganisationId)
            .Select(uor => uor.OrganizationRole.Role.Name)
            .ToListAsync();

            //        var roles = await context.UserOrganizationRoles
            //.AsNoTracking()
            //.Include(uor => uor.OrganizationRole)
            //    .ThenInclude(or => or.Role)
            //.Where(uor => uor.UserId == user.Id && uor.OrganizationRole.OrganizationId == user.OrganisationId)
            //.ToListAsync();
            foreach (var r in roles)
            {
                Console.WriteLine($"Role: {r}");

            }

            var permissions = await context.OrganizationRolePermissions
            .Where(orp => orp.OrganizationRole.OrganizationId == user.OrganisationId && roles.Contains(orp.OrganizationRole.Role.Name))
            .Select(orp => orp.Permission.Name)
            .Distinct()
            .ToListAsync();


            var claims = new List<Claim>
            {
                new Claim("user_id", user.Id.ToString()),
                new Claim("org_id", user.OrganisationId.ToString()),
                new Claim("permissions", string.Join(",", permissions))
            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(7),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
