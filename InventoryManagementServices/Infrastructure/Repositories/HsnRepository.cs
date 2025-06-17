using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class HsnRepository:IHsnCodeRepository
    {
        private readonly AppDbContext _appDbContext;
        public HsnRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
       public async Task<HsnCode> AddHsn(HsnCode hsnCode)
        {
            await _appDbContext.AddAsync(hsnCode);
            await _appDbContext.SaveChangesAsync();
            return hsnCode;

        }
        public async Task<HsnCode> GetByHsnCodeAndOrg(Guid organizationId,int hsnCodeNumber )
        {
            var result= await _appDbContext.HsnCodes
                .FirstOrDefaultAsync(x => !x.IsDeleted
                                          && x.OrgnaisationId == organizationId
                                          && x.HSNCodeNumber == hsnCodeNumber);
            return result;
        }


        public async Task<List<HsnCode>> GetAllHsnCodes(Guid OrganaiztionId)
        {
            return await _appDbContext.HsnCodes.Where(x => !x.IsDeleted &&x.OrgnaisationId==OrganaiztionId).ToListAsync();

        }
       public async Task<HsnCode> GetByHsnCode(int hsnCodeNumber)
        {
             return await _appDbContext.HsnCodes.FirstOrDefaultAsync(x=>!x.IsDeleted&&  x.HSNCodeNumber==hsnCodeNumber);
        }
       public async Task<HsnCode> UpdateHsn(HsnCode hsnCode)
        {
            _appDbContext.Update(hsnCode);
            await _appDbContext.SaveChangesAsync();
            return hsnCode;
        }

       public async Task<bool> DeleteHsnCode(int hsnCode)
        {
            var existinghsnCode = await _appDbContext.HsnCodes
                .FirstOrDefaultAsync(x => x.HSNCodeNumber == hsnCode);

            if (existinghsnCode == null)
                return false;

            existinghsnCode.IsDeleted = true;
            _appDbContext.HsnCodes.Update(existinghsnCode);
            await _appDbContext.SaveChangesAsync();

            return true;
        }
    }
}
