using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Application.Services;
using UserGrpc;


namespace IdentityService.Services
{
    public class UserGrpcService : UserGrpc.UserService.UserServiceBase
    {
        private readonly IUserService _userService;
        public UserGrpcService(IUserService userService)
        {
            _userService = userService;
        }

        public override async Task<GetUserDetailResponse> GetUserDetail(GetUserDetailRequest request, Grpc.Core.ServerCallContext context)
        {
            if (!Guid.TryParse(request.UserId, out var userId))
            {
                return new GetUserDetailResponse
                {
                    StatusCode = 400,
                    Message = "Invalid GUID format",
                    Data = null
                };
            }

            var user = await _userService.GetUsersById(Guid.Parse(request.UserId));

            if (user == null)
            {
                return new GetUserDetailResponse
                {
                    StatusCode = 404,
                    Message = "User not found",
                    Data = null
                };
            }

            return new GetUserDetailResponse
            {
                StatusCode = 200,
                Message = "User found",
                Data = new UserData
                {
                    Name = user.Data.Name,
                    Email = user.Data.Email,
                }
            };
        }
    }
}
