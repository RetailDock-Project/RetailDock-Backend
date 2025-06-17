using Application.Dto;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public interface IUnitOfMeasureServices
    {
        Task<Responses<string>> AddUnitOfMeasure(UnitOfMeasureDto dto);
        Task<Responses<List<GetUnitOfMeasureDto>>> GetAllUnitOfMeasures(Guid OrganaiztionId);
        Task<Responses<GetUnitOfMeasureDto>> GetUnitOfMeasureById(int id);
        Task<Responses<string>> UpdateUnitOfMeasure(int Id,UnitOfMeasureDto dto);
        Task<Responses<string>> DeleteUnitOfMeasure(int id);
    }
    public class UnitOfMeasureServices : IUnitOfMeasureServices
    {
        private readonly IUnitOfMeasureRepository _repository;
        private readonly IMapper _mapper;

        public UnitOfMeasureServices(IUnitOfMeasureRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Responses<string>> AddUnitOfMeasure(UnitOfMeasureDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Measurement))
                {
                    return new Responses<string> { Message = "Measurement name is required", StatusCode = 400 };
                }

                var entity = _mapper.Map<UnitOfMeasures>(dto);
                await _repository.CreateAsync(entity);

                return new Responses<string> { Message = "Unit of Measure added", StatusCode = 201 };
            }
            catch (Exception ex)
            {
                return new Responses<string> { Message = $"Error adding unit: {ex.Message}", StatusCode = 500 };
            }
        }

        public async Task<Responses<List<GetUnitOfMeasureDto>>> GetAllUnitOfMeasures(Guid OrganaiztionId)
        {
            try
            {
                var units = await _repository.GetAllAsync(OrganaiztionId);
                var data = _mapper.Map<List<GetUnitOfMeasureDto>>(units);

                return new Responses<List<GetUnitOfMeasureDto>>
                {
                    Message = "Units fetched successfully",
                    StatusCode = 200,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                return new Responses<List<GetUnitOfMeasureDto>>
                {
                    Message = $"Error fetching units: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<Responses<GetUnitOfMeasureDto>> GetUnitOfMeasureById(int id)
        {
            try
            {
                var unit = await _repository.GetByIdAsync(id);
                if (unit == null)
                {
                    return new Responses<GetUnitOfMeasureDto>
                    {
                        Message = "Unit not found",
                        StatusCode = 404
                    };
                }

                var dto = _mapper.Map<GetUnitOfMeasureDto>(unit);
                return new Responses<GetUnitOfMeasureDto>
                {
                    Data = dto,
                    Message = "Unit fetched",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new Responses<GetUnitOfMeasureDto>
                {
                    Message = $"Error retrieving unit: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<Responses<string>> UpdateUnitOfMeasure(int Id,UnitOfMeasureDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Measurement))
                {
                    return new Responses<string> { Message = "Measurement name is required", StatusCode = 400 };
                }

                var existing = await _repository.GetByIdAsync(Id);
                if (existing == null)
                {
                    return new Responses<string> { Message = "Unit not found", StatusCode = 404 };
                }

                if (existing.IsDeleted)
                {
                    return new Responses<string> { Message = "Cannot update a deleted unit", StatusCode = 400 };
                }

                _mapper.Map(dto, existing);
                await _repository.UpdateAsync(existing);

                return new Responses<string> { Message = "Unit updated", StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new Responses<string> { Message = $"Error updating unit: {ex.Message}", StatusCode = 500 };
            }
        }

        public async Task<Responses<string>> DeleteUnitOfMeasure(int id)
        {
            try
            {
                var existing = await _repository.GetByIdAsync(id);
                if (existing == null)
                {
                    return new Responses<string> { Message = "Unit not found", StatusCode = 404 };
                }

                if (existing.IsDeleted)
                {
                    return new Responses<string> { Message = "Unit already deleted", StatusCode = 400 };
                }

                await _repository.SoftDeleteAsync(id);
                return new Responses<string> { Message = "Unit deleted (soft)", StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new Responses<string> { Message = $"Error deleting unit: {ex.Message}", StatusCode = 500 };
            }
        }
    }
}
