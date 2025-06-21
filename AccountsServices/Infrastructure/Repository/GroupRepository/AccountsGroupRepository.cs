using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.Interfaces.IRepository;
using Application.Services.AccountsService;
using Dapper;
using Domain.Entities;
using Infrastructure.DapperContext;

namespace Infrastructure.Repository.GroupRepository
{
    public class AccountsGroupRepository: IAccountsGroupRepository
    {
        private readonly DapperConection _dapperConection;
        public AccountsGroupRepository(DapperConection dapperConection)
        {
            _dapperConection = dapperConection;
        }
        public async Task<bool> createGroupDefault(Guid OrganizationId, Guid CreatedBy)
        {
            var sql = @"Call sp_CreateDefaultAccountsGroups(@OrganizationId, @CreatedBy);";
            using var connection = _dapperConection.CreateConnection();
            var roweffected = await connection.ExecuteAsync(sql, new
            {
                OrganizationId = OrganizationId,
                CreatedBy = CreatedBy
            });
            return roweffected > 0;

        }
        public async Task<bool> AddParentGroup(Guid OrganizationId, AddParentGroupDTO addGroupDTO)
        {
            var sql = @"INSERT INTO AccountsGroups(Id, OrganizationId, GroupName,
                 AccountsMasterGroupId, CreatedBy, UpdatedBy, Nature) Values
              (@Id, @OrganizationId, @GroupName, @AccountsMasterGroupId, @CreatedBy, @UpdatedBy,@Nature)";
            var group = new
            {
                Id = Guid.NewGuid(),
                OrganizationId = OrganizationId,
                GroupName = addGroupDTO.GroupName,
                AccountsMasterGroupId = addGroupDTO.AccountsMasterGroupId,
                CreatedBy = addGroupDTO.CreatedBy,
                UpdatedBy = addGroupDTO.UpdatedBy,
                Nature=addGroupDTO.Nature
            };
            try
            {
                using var connection = _dapperConection.CreateConnection();
                var result = await connection.ExecuteAsync(sql, group);
                return result > 0;
            }
            catch (Exception ex)
            {
             
                Console.WriteLine($"Connection error: {ex.Message}");
                throw;
            }
        }
        public async Task<bool> AddSubGroup(Guid OrganizationId, AddSubGroupDTO addGroupDTO)
        {
            var sql = @"INSERT INTO AccountsGroups(Id, OrganizationId, GroupName,
                 ParentId, CreatedBy, UpdatedBy,Nature) Values
              (@Id, @OrganizationId, @GroupName, @ParentId, @CreatedBy, @UpdatedBy,@Nature)";
            var group = new
            {
                Id = Guid.NewGuid(),
                OrganizationId = OrganizationId,
                GroupName = addGroupDTO.GroupName,
                ParentId = addGroupDTO.ParentId,
                CreatedBy = addGroupDTO.CreatedBy,
                UpdatedBy = addGroupDTO.UpdatedBy,
                Nature=addGroupDTO.Nature
              

            };

            using var connection = _dapperConection.CreateConnection();
            var result = await connection.ExecuteAsync(sql, group);
            return result > 0;
        }
        public async Task<List<GetParentGroupsDTO>> GetParentGroups(Guid organizationId)
        {
            var sql = @"SELECT Id, GroupName 
                FROM AccountsGroups 
                WHERE AccountsMasterGroupId IS NOT NULL 
                AND (OrganizationId IS NULL OR OrganizationId = @OrganizationId)";

            using var connection = _dapperConection.CreateConnection();
            var result = await connection.QueryAsync<GetParentGroupsDTO>(sql, new { OrganizationId = organizationId });
            return result.ToList();
        }
        public async Task<List<GetSubGroupsDTO>> GetSubGroups(Guid organizationId)
        {
            var sql = @"SELECT Id, GroupName 
                FROM AccountsGroups 
                WHERE ParentId IS NOT NULL 
                AND (OrganizationId IS NULL OR OrganizationId = @OrganizationId)";

            using var connection = _dapperConection.CreateConnection();
            var result = await connection.QueryAsync<GetSubGroupsDTO>(sql, new { OrganizationId = organizationId });
            return result.ToList();
        }
        public async Task<bool> IsGroupNameExists(Guid organizationId, string groupName)
        {
            var sql = @"
        SELECT COUNT(*) 
        FROM AccountsGroups 
        WHERE LOWER(GroupName) = LOWER(@GroupName) 
          AND (OrganizationId = @OrganizationId OR OrganizationId IS NULL)";

            using var connection = _dapperConection.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(sql, new { GroupName = groupName, OrganizationId = organizationId });
            return count > 0;
        }

       
    }
}
