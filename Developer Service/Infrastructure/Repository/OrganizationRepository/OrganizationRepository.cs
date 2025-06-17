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
        public async Task<Decimal> TotalSubscriptionReceivedBySpecificDate(DateTime FromDate, DateTime ToDate)
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
        public  async Task<Decimal> TotalSubscriptionReceivedByCurrentYear()
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
        public async Task<Decimal> TotalSubscriptionReceivedByCurrentMonth()
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
            data.IsActive = false;
            data.UpdatedAt = DateTime.Now;
             _context.OrganizationDetail.Update(data);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
