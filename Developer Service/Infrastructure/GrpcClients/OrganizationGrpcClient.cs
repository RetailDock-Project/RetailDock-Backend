using Application.Interfaces.IRepository;
using Grpc.Net.Client;
using GrpcContracts;
using Microsoft.Extensions.Logging;
using GroupGrpc;

// In Infrastructure/Grpc/OrganizationGrpcClient.cs
public class OrganizationGrpcClient : IOrganizationNotifierGrpcService
{
    private readonly GroupGrpc.AccountsGroupService.AccountsGroupServiceClient _client;

    public OrganizationGrpcClient(AccountsGroupService.AccountsGroupServiceClient client)
    {
        _client = client;
    }

    public async Task NotifyOrganizationCreated(Guid organizationId, Guid userId)
    {
        var request = new CreateDefaultGroupsRequest
        {
            OrganizationId = organizationId.ToString(),
            CreatedBy = userId.ToString()
        };

        await _client.CreateDefaultGroupsAsync(request);
    }
}

