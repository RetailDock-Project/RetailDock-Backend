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
                RoleId = 4,
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
                return new ResponseDto<List<OrganizationUserDto>> { StatusCode = 200 ,Message="No users found under organization"};
            }
            return new ResponseDto<List<OrganizationUserDto>> { StatusCode = 200 ,Message=$"Users under organization-{orgId} retrieved",Data=result};
        }
    }
}
