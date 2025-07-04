using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using AutoMapper;
using Common;
using Domain.Entities;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepo;
        private readonly IMapper mapper;


        public UserService(IUserRepository _userRepo, IMapper _mapper)
        {
            userRepo = _userRepo;
            mapper = _mapper;

        }

        public async Task UpdateOrganizationIdandRole(Guid userId, Guid organizationId)
        {
            var user = await userRepo.UpdateUserOrganization(userId, organizationId);
            Console.WriteLine("updated user added");
            var newRole = new OrgRoleDto
            {
                OrganizationId = organizationId,
            };
            var mappedOrgRole = mapper.Map<OrganizationRole>(newRole);
            Guid orgRoleId = Guid.NewGuid();
            mappedOrgRole.Id = orgRoleId;
            await userRepo.AddOrganizationRole(mappedOrgRole);
            var userOrgRole = new UserOrgRole {
                OrganizationRoleId = orgRoleId,
                UserId = userId,
            };
            var mappedUserOrgRole = mapper.Map<UserOrganizationRole>(userOrgRole);
            await userRepo.AddUserOrgRole(mappedUserOrgRole);

        }


        public async Task<ResponseDto<List<OrganizationUserDto>>> GetUsersByOrgId(Guid orgId) {
            var result = await userRepo.GetUsersByOrgId(orgId);
            if (result == null || !result.Any()) {
                return new ResponseDto<List<OrganizationUserDto>> { StatusCode = 404 ,Message="No users found under organization"};
            }
            return new ResponseDto<List<OrganizationUserDto>> { StatusCode = 200 ,Message=$"Users under organization-{orgId} retrieved",Data=result};
        }

        public async Task<ResponseDto<UserDto>> GetUsersById(Guid userId)
        {
            var result = await userRepo.GetUserById(userId);
            var user=mapper.Map<UserDto>(result);
            if (result == null)
            {
                return new ResponseDto<UserDto> { StatusCode = 404, Message = "No user found" };
            }
            return new ResponseDto<UserDto> { StatusCode = 200, Message = $"User retrieved", Data = user };
        }

        public async Task<ResponseDto<object>> UpdateUserOrganizationRoleAsync(UpdateUserRoleDto dto)
        {
            var userOrgRole = await userRepo.GetUserOrganizationRoleAsync(dto.UserId);

            if (userOrgRole == null)
            {
                return new ResponseDto<object>
                {
                    StatusCode = 404,
                    Message = "User role not found"
                };
            }

            // Update the role
            userOrgRole.OrganizationRoleId = dto.NewRoleId;
            userOrgRole.UpdatedAt=DateTime.UtcNow;

            await userRepo.UpdateUserOrganizationRoleAsync(userOrgRole);

            return new ResponseDto<object>
            {
                StatusCode = 200,
                Message = "User organization role updated successfully"
            };
        }

        public async Task<ResponseDto<object>> SoftDeleteUserAsync(Guid userId, Guid orgId)
        {
            var user = await userRepo.GetUserById(userId);

            if (user == null || user.IsDeleted || user.OrganisationId != orgId)
            {
                return new ResponseDto<object>
                {
                    StatusCode = 404,
                    Message = "User not found"
                };
            }

            user.IsDeleted = true;

            await userRepo.UpdateUserAsync(user);

            return new ResponseDto<object>
            {
                StatusCode = 200,
                Message = "User soft deleted successfully"
            };
        }


    }
}
