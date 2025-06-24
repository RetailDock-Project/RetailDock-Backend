using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Application.Interfaces.IRepository;
using Domain.Entities;
using UserGrpc;

namespace Infrastructure.GrpcClient
{
    public class UserGrpcClient:IUserGrpcClient
    {
        private readonly UserService.UserServiceClient _client;

        public UserGrpcClient(UserService.UserServiceClient client)
        {
            _client = client;
        }

        public async Task<Responses<UserDto>> GetUserDetailAsync(string userId)
        {
            var request = new GetUserDetailRequest { UserId = userId };
            var response= await _client.GetUserDetailAsync(request);
            if (response.StatusCode != 200) {
                return new Responses<UserDto> { StatusCode = response.StatusCode, Message = response.Message };
            }
            return new Responses<UserDto> { StatusCode = response.StatusCode, Message = response.Message, Data = new UserDto { Name = response.Data.Name, Email = response.Data.Email } };
        }
    }
}
