using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.Interfaces.IRepository;
using Application.Interfaces.IServices;
using Common;
using Microsoft.Extensions.Logging;
using Serilog.Sinks.SystemConsole.Themes;

namespace Application.Services.AccountsService
{
    public class AccountsGroupServices : IAccountsGroupService
    {
        private readonly IAccountsGroupRepository _accountsGroupRepository;
        private readonly ILedgerRepository _ledgerRepository;
        private readonly ILogger<AccountsGroupServices> _logger;
        public AccountsGroupServices(IAccountsGroupRepository accountsGroupRepository, ILogger<AccountsGroupServices> logger, ILedgerRepository ledgerRepository)
        {
            _accountsGroupRepository = accountsGroupRepository;
            _logger = logger;
            _ledgerRepository = ledgerRepository;
        }
        public async Task<ApiResponseDTO<object>> CreateParentGroup(Guid OrganizationId, AddParentGroupDTO addParentGroupDTO)
        {
            try

            {
                var IsExisting = await _accountsGroupRepository.IsGroupNameExists(OrganizationId, addParentGroupDTO.GroupName);
                if (IsExisting)
                {
                    return new ApiResponseDTO<object>
                    {
                    StatusCode=400,
                    Message="The Group name is already exist"
                    
                    };

                }
                if (string.IsNullOrWhiteSpace(addParentGroupDTO.GroupName?.Trim()))
                {
                    return new ApiResponseDTO<object>
                    {
                        StatusCode = 400,
                        Message = "GroupName cannot be empty or whitespace."
                    };
                }
                var nature = await _ledgerRepository.GetNatureByGroupIdOrMasterGroupIdAsync(addParentGroupDTO.AccountsMasterGroupId);
                if (nature == null)
                {
                    return new ApiResponseDTO<object>
                    {
                        StatusCode = 404,
                        Message = "Group Not found"
                    };
                }
                addParentGroupDTO.Nature = nature;
                var result = await _accountsGroupRepository.AddParentGroup(OrganizationId, addParentGroupDTO);
                if (result)
                {
                    return new ApiResponseDTO<object>
                    {
                        StatusCode = 201,
                        Message = "Group Added Succussfully",

                    };

                }
                return new ApiResponseDTO<object>
                {
                    StatusCode = 404,
                    Message = "Organization not found"
                };


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error in add a group");
                return new ApiResponseDTO<object>
                {
                    StatusCode = 500,
                    Message = "Error in add a group"

                };
            }
        }
        public async Task<ApiResponseDTO<object>> CreateSubGroup(Guid OrganizationId, AddSubGroupDTO addSubGroupDTO)
        {
            try
            {
                var IsExisting = await _accountsGroupRepository.IsGroupNameExists(OrganizationId, addSubGroupDTO.GroupName);
                if (IsExisting)
                {
                    return new ApiResponseDTO<object>
                    {
                        StatusCode = 400,
                        Message = "The Sub Group name is already exist"

                    };

                }
                if (string.IsNullOrWhiteSpace(addSubGroupDTO.GroupName?.Trim()))
                {
                    return new ApiResponseDTO<object>
                    {
                        StatusCode = 400,
                        Message = "GroupName cannot be empty or whitespace."
                    };
                }
                var nature = await _ledgerRepository.GetNatureByGroupIdOrMasterGroupIdAsync(addSubGroupDTO.ParentId);
                if (nature == null)
                {
                    return new ApiResponseDTO<object>
                    {
                        StatusCode = 404,
                        Message = "Group Not found"
                    };
                }
                addSubGroupDTO.Nature = nature;
                var result = await _accountsGroupRepository.AddSubGroup(OrganizationId, addSubGroupDTO);
                if (result)
                {
                    return new ApiResponseDTO<object>
                    {
                        StatusCode = 201,
                        Message = "sub Group Added Succussfully",

                    };

                }
                return new ApiResponseDTO<object>
                {
                    StatusCode = 404,
                    Message = "Organization not found"
                };


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error in add a group");
                return new ApiResponseDTO<object>
                {
                    StatusCode = 500,
                    Message = "Error in add a group"

                };
            }

        }
        public async Task<ApiResponseDTO<List<GetSubGroupsDTO>>> GetSubGroups(Guid organizationId)
        {
            try
            {
                var result = await _accountsGroupRepository.GetSubGroups(organizationId);
                if (result.Count > 0)
                {
                    return new ApiResponseDTO<List<GetSubGroupsDTO>>
                    {
                        StatusCode = 200,
                        Message = "Succussfully Fetched",
                        Data = result
                    };

                }
                return new ApiResponseDTO<List<GetSubGroupsDTO>>
                {
                    StatusCode = 404,
                    Message = "Organization not found"
                };


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error in Fetching groups");
                return new ApiResponseDTO<List<GetSubGroupsDTO>>
                {
                    StatusCode = 500,
                    Message = "Error in Fetching groups"

                };
            }

        }
        public async Task<ApiResponseDTO<List<GetParentGroupsDTO>>> GetParentGroups(Guid organizationId)
        {
            try
            {
                var result = await _accountsGroupRepository.GetParentGroups(organizationId);
                if (result.Count > 0)
                {
                    return new ApiResponseDTO<List<GetParentGroupsDTO>>
                    {
                        StatusCode = 200,
                        Message = "Succussfully Fetched",
                        Data = result
                    };

                }
                return new ApiResponseDTO<List<GetParentGroupsDTO>>
                {
                    StatusCode = 404,
                    Message = "Organization not found"
                };


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error in Fetching groups");
                return new ApiResponseDTO<List<GetParentGroupsDTO>>
                {
                    StatusCode = 500,
                    Message = "Error in Fetching Parent groups"

                };
            }
        }
        public async Task<ApiResponseDTO<bool>> CreateDefaultGroups(Guid organizationId, Guid createdBy)
        {
            try
            {
                var result = await _accountsGroupRepository.createGroupDefault(organizationId, createdBy);
                if (!result)
                {
                    return new ApiResponseDTO<bool>
                    {
                        StatusCode = 400,
                        Message = "Group creation failed",
                        Data = false
                    };
                }

                return new ApiResponseDTO<bool>
                {
                    StatusCode = 200,
                    Message = "Default groups created successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating default groups");
                return new ApiResponseDTO<bool>
                {
                    StatusCode = 500,
                    Message = "Internal server error",
                    Data = false
                };
            }
        }
        

    }
}
