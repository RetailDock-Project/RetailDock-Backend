using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Entities;
using Microsoft.VisualBasic;

namespace Application.Interfaces.IRepository
{
    public interface IOrganizationRepository
    {
        Task<bool> CheckUserIdIsAlreadyExistInOrganization(Guid userId);
        Task<bool> AddCompanyWithSubscriptionAsync(OrganizationDetails org, Subscriptions sub);
        Task<int> TotalOrganizationCount();
        Task<decimal> TotalSubscriptionReceivedByCurrentMonth();
        Task<decimal> TotalSubscriptionReceivedByCurrentYear();
        Task<decimal> TotalSubscriptionReceivedBySpecificDate(DateTime FromDate, DateTime ToDate);
        Task<object> GetOrganizationAccountStatusSummaryAsync();
        Task<List<OrganizationDetails>> GetAllOrganizationWithSubscription();
        Task<bool>BlockOrganization(Guid organizationId);
        Task<OrganizationDetails> GetOrganizationDetailById(Guid id);

        Task<List<OrganizationSignupChartDto>> GetOrganizationSignupLast7MonthsAsync();
        Task<List<MonthlyRevenueDto>> GetLast7MonthsRevenueAsync();

        Task<List<OrganizationListDto>> GetAllOrganizationsAsync(string? search, string? status);
        Task<OrganizationDetails?> GetByIdAsync(Guid orgId);




    }
}
