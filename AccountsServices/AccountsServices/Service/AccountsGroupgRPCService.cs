
﻿using Application.Interfaces.IServices;
using Application.Services.AccountsService;
using GroupGrpc; 
using Grpc.Core;

namespace AccountsServices.Service
{
    public class AccountsGroupgRPCService : AccountsGroupService.AccountsGroupServiceBase
    {
        private readonly IAccountsGroupService _accountsGroupService;
        private readonly ILogger<AccountsGroupgRPCService> _logger;

        public AccountsGroupgRPCService(IAccountsGroupService accountsGroupService, ILogger<AccountsGroupgRPCService> logger)
        {
            _accountsGroupService = accountsGroupService;
            _logger = logger;
        }

        public override async Task<ApiResponse> CreateDefaultGroups(CreateDefaultGroupsRequest request, ServerCallContext context)
        {
            try
            {
                var organizationId = Guid.Parse(request.OrganizationId);
                var createdBy = Guid.Parse(request.CreatedBy);



                var result = await _accountsGroupService.CreateDefaultGroups(organizationId, createdBy);

                return new ApiResponse
                {
                    StatusCode = result.StatusCode,
                    Message = result.Message ?? string.Empty,
                    Data = result.Data
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateDefaultGroups");
                return new ApiResponse
                {
                    StatusCode = 500,
                    Message = "Internal Server Error",
                    Data = false
                };
            }
        }
    }
}

