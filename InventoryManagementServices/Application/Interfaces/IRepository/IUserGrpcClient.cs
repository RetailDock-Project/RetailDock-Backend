using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Domain.Entities;

namespace Application.Interfaces.IRepository
{
    public interface IUserGrpcClient
    {
        Task<Responses<UserDto>> GetUserDetailAsync(string userId);
    }
}
