using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUnitOfMeasureRepository
    {
        Task<IEnumerable<UnitOfMeasures>> GetAllAsync(Guid OrganaiztionId);
        Task<UnitOfMeasures> GetByIdAsync(int id);
        Task<UnitOfMeasures> CreateAsync(UnitOfMeasures entity);
        Task<UnitOfMeasures> UpdateAsync(UnitOfMeasures entity);
        Task<bool> SoftDeleteAsync(int id);
    }

}
