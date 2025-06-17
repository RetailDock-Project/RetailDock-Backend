using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UnitOfMeasureRepository : IUnitOfMeasureRepository
    {
        private readonly AppDbContext _context;

        public UnitOfMeasureRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UnitOfMeasures>> GetAllAsync(Guid OrganaiztionId)
        {
            return await _context.UnitOfMeasures
                                 .Where(x => !x.IsDeleted &&x.OrgnaisationId==OrganaiztionId)
                                 .ToListAsync();
        }

        public async Task<UnitOfMeasures> GetByIdAsync(int id)
        {
            return await _context.UnitOfMeasures
                                 .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task<UnitOfMeasures> CreateAsync(UnitOfMeasures entity)
        {
            _context.UnitOfMeasures.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<UnitOfMeasures> UpdateAsync(UnitOfMeasures entity)
        {
            _context.UnitOfMeasures.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var unit = await _context.UnitOfMeasures.FindAsync(id);
            if (unit == null || unit.IsDeleted) return false;

            unit.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

