using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Dto;
using Application.Interfaces.IServices;
using Domain.Entities;

namespace Application.Services
{
    public class OrganizationApiService : IOrganizationService
    {
        private readonly HttpClient _httpClient;

        public OrganizationApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<OrganizationDetailsDto?> GetOrganizationByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/Organization/detail/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Responses<OrganizationDetailsDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result?.Data;
            }

            return null;
        }

    }
    }
