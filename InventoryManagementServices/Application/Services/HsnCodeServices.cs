using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.VisualBasic;

namespace Application.Services
{
    public interface IHsnCodeServices
    {
        Task<Responses<string>> AddHsn(HsnDto hsnDto);
        Task<Responses<List<HsnDto>>> GetAllHsn(Guid OrganaiztionId);
        Task<Responses<HsnDto>> Getbyhsncode(int hsncode);
        Task<Responses<string>>UpdateHsn(int hsnCode, UpdateHsnDto hsnDto);
        Task<Responses<string>> DeleteHsn(int hsncode);
    }
    public class HsnCodeServices : IHsnCodeServices
    {
        private readonly IMapper _mapper;
        private readonly IHsnCodeRepository _hsnCodeRepository;

        public HsnCodeServices(IMapper mapper, IHsnCodeRepository hsnCodeRepository)
        {
            _mapper = mapper;
            _hsnCodeRepository = hsnCodeRepository;
        }

        public async Task<Responses<string>> AddHsn(HsnDto hsnDto)
        {
            try
            {
                var existingHsn = await _hsnCodeRepository.GetByHsnCodeAndOrg(hsnDto.OrgnaisationId, hsnDto.HSNCodeNumber);

                if (existingHsn != null)
                {
                    return new Responses<string>
                    {
                        Message = "HSN code already exists.",
                        StatusCode = 409 
                    };
                }
                var hsn = _mapper.Map<HsnCode>(hsnDto);
                await _hsnCodeRepository.AddHsn(hsn);
                return new Responses<string> { Message = "HsnCode And TaxRate Added", StatusCode = 201 };
            }
            catch (Exception ex)
            {
                return new Responses<string> { StatusCode = 500, Message = ex.Message };
            }
        }

        public async Task<Responses<List<HsnDto>>> GetAllHsn(Guid OrganaiztionId)
        {
            try
            {
                var hsn = await _hsnCodeRepository.GetAllHsnCodes(OrganaiztionId);
                
                var mapped = _mapper.Map<List<HsnDto>>(hsn);
                return new Responses<List<HsnDto>> { Data = mapped, Message = "All HsnCode&Gst Fetched", StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new Responses<List<HsnDto>> { StatusCode = 500, Message = ex.Message };
            }
        }

        public async Task<Responses<HsnDto>> Getbyhsncode(int hsncode)
        {
            try
            {
                var hsn = await _hsnCodeRepository.GetByHsnCode(hsncode);
                if (hsn == null)
                {
                    return new Responses<HsnDto> { Message = "HsnCode not Found", StatusCode = 404 };
                }
                var mapped = _mapper.Map<HsnDto>(hsn);
                return new Responses<HsnDto> { Data = mapped, StatusCode = 200, Message = "HsnCode&Gst  fetched" };
            }
            catch (Exception ex)
            {
                return new Responses<HsnDto> { Message = ex.Message, StatusCode = 400 };

            }
        }

        public async Task<Responses<string>> UpdateHsn(int hsnCode, UpdateHsnDto hsnDto)
        {
            try
            {
                var hsn = await _hsnCodeRepository.GetByHsnCode(hsnCode);
                if (hsn == null)
                {
                    return new Responses<string> { Message = "Hsn code not found", StatusCode = 400 };
                }
                _mapper.Map(hsnDto, hsn);
                await _hsnCodeRepository.UpdateHsn(hsn);
                return new Responses<string> { Message = "GstRate Updated", StatusCode = 200 };


            }
            catch (Exception ex)
            {
                return new Responses<string> { Message = ex.Message, StatusCode = 500 };
            }
        }

        public async Task<Responses<string>> DeleteHsn(int hsncode)
        {
            try
            {
                var hsn = await _hsnCodeRepository.GetByHsnCode(hsncode);
                if (hsn == null)
                {
                    return new Responses<string>
                    {
                        Message = "Hsn code not found",
                        StatusCode = 404
                    };
                }

                bool deleted = await _hsnCodeRepository.DeleteHsnCode(hsncode);
                if (!deleted)
                {
                    return new Responses<string>
                    {
                        Message = "Failed to delete Hsn code",
                        StatusCode = 500
                    };
                }

                return new Responses<string>
                {
                    Message = "Hsn code deleted successfully",
                    StatusCode = 200,
                    Data = hsncode.ToString()
                };
            }
            catch (Exception ex)
            {
                return new Responses<string>
                {
                    Message = $"Internal error: {ex.Message}",
                    StatusCode = 500
                };
            }
        }
    }
    }
