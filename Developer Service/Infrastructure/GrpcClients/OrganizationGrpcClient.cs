using Application.Interfaces.IRepository;
using Grpc.Net.Client;
using GrpcContracts;
using Microsoft.Extensions.Logging;
using OrganizationAccounting;

// In Infrastructure/Grpc/OrganizationGrpcClient.cs
public class OrganizationGrpcClient : IOrganizationNotifierGrpcService
{
    private readonly OrganizationNotifier.OrganizationNotifierClient _client;

    public OrganizationGrpcClient(OrganizationNotifier.OrganizationNotifierClient client)
    {
        _client = client;
    }

    public async Task NotifyOrganizationCreated(Guid organizationId, Guid userId)
    {
        var request = new OrganizationCreatedRequest
        {
            OrganizationId = organizationId.ToString(),
            UserId = userId.ToString()
        };

        await _client.NotifyOrganizationCreatedAsync(request);
    }
}

