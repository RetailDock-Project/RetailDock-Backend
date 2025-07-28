using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.IRepository;
using Domain.Entities;
using Infrastructure.AppDbContext;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Infrastructure.Repository.OrganizationRepository
{
    public class OrganizationRepository:IOrganizationRepository
    {
        private readonly ApplicationDbContext _context;
        public OrganizationRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CheckUserIdIsAlreadyExistInOrganization(Guid userId)
        {
            return await _context.OrganizationDetail
                .AnyAsync(o => o.UserId == userId);
        }

        public async Task<bool> AddCompanyWithSubscriptionAsync(OrganizationDetails org, Subscriptions sub)
        {

            using var transaction=_context.Database.BeginTransaction();
            try
            {
                await _context.OrganizationDetail.AddAsync(org);
                await _context.Subscription.AddAsync(sub);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Log.Error(ex, "Transaction failed while inserting organization and subscription.");
                return false;
            }
            
        }
        public async Task<int> TotalOrganizationCount()
        {
            try
            {
                return await _context.OrganizationDetail.CountAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while counting total organizations.");
                throw new Exception("Failed to get total organization count.");
            }
        }
        public async Task<decimal> TotalSubscriptionReceivedBySpecificDate(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                return await _context.Subscription.Where(x => x.UpdatedAt.Date >= FromDate && x.UpdatedAt.Date <= ToDate).SumAsync(s => s.Amount);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while getting  total Amount By Specific Date");
                throw new Exception("Failed to get get total Amount");
            }
        }
        public  async Task<decimal> TotalSubscriptionReceivedByCurrentYear()
        {
            try
            {
                var currentYear = DateTime.Now.Year;
                return await _context.Subscription.Where(x => x.UpdatedAt.Year == currentYear).SumAsync(s => s.Amount);

            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while getting  total Amount By year");
                throw new Exception("Failed to get get total Amount by year");
            }

           
        }
        public async Task<decimal> TotalSubscriptionReceivedByCurrentMonth()
        {
            try
            {
                var currentMonth = DateTime.Now.Month;
                return await _context.Subscription.Where(x => x.UpdatedAt.Month == currentMonth).SumAsync(s => s.Amount);

            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while getting  total Amount By year");
                throw new Exception("Failed to get get total Amount by year");
            }


        }
        public async Task<object> GetOrganizationAccountStatusSummaryAsync()
        {
            var currentDate = DateTime.Now;

            var activeCount = await _context.OrganizationDetail
                .CountAsync(o => o.IsActive == true);

            var inactiveCount = await _context.OrganizationDetail
                .CountAsync(o => o.IsActive == false);

            var expiredCount = await _context.OrganizationDetail
           .Where(o => o.IsActive) // Only include active orgs
          .Where(o =>o.Subscriptions.ExpiryDate < currentDate)
          .CountAsync();

            return new
            {
                ActiveCount = activeCount,
                InactiveCount = inactiveCount,
                ExpiredCount = expiredCount
            };
        }
       public async  Task<List<OrganizationDetails>> GetAllOrganizationWithSubscription()
        {
            return await _context.OrganizationDetail.Include(x=>x.Subscriptions).ToListAsync();
        }
       public async Task<bool> BlockOrganization(Guid organizationId)
        {
             var data=await _context.OrganizationDetail.FirstOrDefaultAsync(x => x.OrganizationId == organizationId);
            if (data == null) 
            {
            return false;
            }
            data.IsActive = !data.IsActive;
            data.UpdatedAt = DateTime.Now;
             _context.OrganizationDetail.Update(data);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<OrganizationDetails> GetOrganizationDetailById(Guid id) {
            return await _context.OrganizationDetail.FirstOrDefaultAsync(org => org.OrganizationId == id);
        }


        public async Task<List<OrganizationSignupChartDto>> GetOrganizationSignupLast7MonthsAsync()
        {
            DateTime today = DateTime.UtcNow;
            DateTime startDate = today.AddMonths(-6); // Last 7 months including current

            var result = await _context.OrganizationDetail
                .Where(o => o.CreatedAt >= startDate)
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new OrganizationSignupChartDto
                {
                    Name = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM"),
                    Users = g.Count()
                })
                .ToListAsync();

            return result;
        }

        public async Task<List<MonthlyRevenueDto>> GetLast7MonthsRevenueAsync()
        {
            DateTime today = DateTime.UtcNow;
            DateTime startDate = today.AddMonths(-6); // Include current month + 6 before

            var result = await _context.Subscription
                .Where(s => s.CreatedAt >= startDate)
                .GroupBy(s => new { s.CreatedAt.Year, s.CreatedAt.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new MonthlyRevenueDto
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM"),
                    Revenue = g.Sum(s => s.Amount)
                })
                .ToListAsync();

            return result;
        }


        public async Task<List<OrganizationListDto>> GetAllOrganizationsAsync(string? search, string? status)
        {
            var query = _context.OrganizationDetail
                .Include(o => o.Subscriptions)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(o => o.OrganizationName.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                switch (status.ToLower())
                {
                    case "active":
                        query = query.Where(o => o.IsActive && o.Subscriptions.ExpiryDate >= DateTime.UtcNow);
                        break;
                    case "expired":
                        query = query.Where(o => o.Subscriptions.ExpiryDate < DateTime.UtcNow);
                        break;
                    case "inactive":
                        query = query.Where(o => !o.IsActive);
                        break;
                }
            }

            var result = await query
                .Select(o => new OrganizationListDto
                {
                    OrganizationId = o.OrganizationId,
                    OrganizationName = o.OrganizationName,
                    PlanName = o.Subscriptions.SubscriptionName,
                    SignUpDate = o.CreatedAt,
                    ExpiryDate = o.Subscriptions.ExpiryDate,
                    PlanStatus = o.Subscriptions.ExpiryDate >= DateTime.UtcNow
                                    ? "Active"
                                    : "Expired",
                    IsActive = o.IsActive,
                })
                .ToListAsync();

            return result;
        }

        public async Task<OrganizationDetails?> GetByIdAsync(Guid orgId)
        {
            return await _context.OrganizationDetail
                .Include(o => o.Subscriptions)
                .FirstOrDefaultAsync(o => o.OrganizationId == orgId);
        }


    }
}
