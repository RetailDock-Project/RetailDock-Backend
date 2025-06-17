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
        Task<Decimal> TotalSubscriptionReceivedByCurrentMonth();
        Task<Decimal> TotalSubscriptionReceivedByCurrentYear();
        Task<Decimal> TotalSubscriptionReceivedBySpecificDate(DateTime FromDate, DateTime ToDate);
        Task<object> GetOrganizationAccountStatusSummaryAsync();
        Task<List<OrganizationDetails>> GetAllOrganizationWithSubscription();
        Task<bool>BlockOrganization(Guid organizationId);

    }
}
