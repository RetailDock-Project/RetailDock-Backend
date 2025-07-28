using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Common.ApiResponse;
using Domain.Entities;

namespace Application.Interfaces.IService
{
    public interface IOrganizationServices
    {
        Task<ApiResponseDTO<string>> AddSubscription(AddSubscriptionDTO addSubscriptionDTO, Guid UserId);
       
        Task<ApiResponseDTO<int>> GetTotalCountOfOrganization();
        Task<ApiResponseDTO<Decimal>> TotalSubscriptionReceivedByCurrentMonth();
        Task<ApiResponseDTO<Decimal>> TotalSubscriptionReceivedByCurrentYear();
        Task<ApiResponseDTO<Decimal>> TotalSubscriptionReceivedBySpecificDate(DateTime FromDate, DateTime ToDate);
        Task<ApiResponseDTO<object>> GetOrganizationAccountStatusSummaryAsync();
        Task<ApiResponseDTO<List<GetAllOrganizatinsDTO>>> GetAllOrganizationWithSubscription();
        Task<ApiResponseDTO<bool>> BlockOrganization(Guid organizationId);
        Task<ApiResponseDTO<OrganizationDetailsDto>> GetOrganizationDetailById(Guid id);
        //microservice
        Task<ApiResponseDTO<DashboardSummaryDto>> GetDashboardSummary();
        Task<ApiResponseDTO<List<OrganizationSignupChartDto>>> GetOrganizationSignupLast7MonthsAsync();
        Task<ApiResponseDTO<List<MonthlyRevenueDto>>> GetLast7MonthsRevenueAsync();
        Task<ApiResponseDTO<List<OrganizationListDto>>> GetAllOrganizationsAsync(string? search, string? status);

        Task<ApiResponseDTO<OrganizationResponseDTO>> GetOrganizationDetailsAsync(Guid orgId);



    }
}

