using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class MapperProfile:Profile
    {
        public MapperProfile() {

            CreateMap<RoleAddDto, Role>();
            CreateMap<AddUserDto, User>();
            CreateMap<Role, RoleDto>().ReverseMap();
            CreateMap<OrganizationRole, GetOrgRoleDto>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Role.Name)).ReverseMap();
            CreateMap<OrganizationRole, OrgRoleDto>().ReverseMap();
            CreateMap<OrgRolePermissionDto, OrganizationRolePermission>().ReverseMap();
            CreateMap<OrganizationRolePermission, GetOrgRolePermissionDto>().ForMember(dest=>dest.PermissionName,opt=>opt.MapFrom(src=>src.Permission.Name)).ReverseMap();
            CreateMap<UserOrgRole, UserOrganizationRole>().ReverseMap();
            CreateMap<PermissionDto, Permission>().ReverseMap();
            CreateMap<User, OrganizationUserDto>().ForMember(dest=>dest.Name,opt=>opt.MapFrom(src=>src.Name)).ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email)).ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.userOrganizationRole.Any()));

        }
    }
}
