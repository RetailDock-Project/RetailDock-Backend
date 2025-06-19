using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Events;
using Application.Interfaces.IRepository;
using Application.Interfaces.IService;
using AutoMapper;
using Common.ApiResponse;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Services.OrganizationService
{
    public class OrganizationService : IOrganizationServices
    {
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OrganizationService> _logger;
        private readonly IRabbitMQProducer _producer;
        private readonly IOrganizationNotifierGrpcService _notifierGrpc;



        public OrganizationService(IOrganizationRepository organizationRepository, ILogger<OrganizationService> logger, IRabbitMQProducer producer, IOrganizationNotifierGrpcService notifierGrpc)
        {
            _organizationRepository = organizationRepository;
            _notifierGrpc = notifierGrpc;

            _logger = logger;
            _producer = producer;
        }
        public async Task<ApiResponseDTO<string>> AddSubscription(AddSubscriptionDTO addSubscriptionDTO, Guid UserId)
        {
            
            try
            {
                var isExisting = await _organizationRepository.CheckUserIdIsAlreadyExistInOrganization(UserId);
                if (isExisting)
                {
                    return new ApiResponseDTO<string>
                    {
                        StatusCode = 400,
                        Message = "User Have already a organization"

                    };
                }
                DateOnly fyStart;
                DateOnly fyEnd;

                if (addSubscriptionDTO.FinancialYearStart.ToLower() == "jantodec")
                {
                    fyStart = new DateOnly(1, 1, 1);
                    fyEnd = new DateOnly(1, 12, 31);
                }
                else if (addSubscriptionDTO.FinancialYearStart.ToLower() == "apriltomarch")
                {
                    fyStart = new DateOnly(1, 4, 1);
                    fyEnd = new DateOnly(1, 3, 31);
                }
                else
                {
                    Log.Error("Invalid FinancialYearStart value. Use 'jantodec' or 'apriltomarch'.");

                    return new ApiResponseDTO<string>
                    {

                        StatusCode = 400,
                        Message = "Error occurred while creating subscription for user { UserId}",
                        Data = null
                    };
                }

                var newOrganization = new OrganizationDetails
                {
                    OrganizationId = Guid.NewGuid(),
                    OrganizationName= addSubscriptionDTO.OrganizationName,
                    Address = addSubscriptionDTO.Address,
                    PANNumber = addSubscriptionDTO.PANNumber,
                    GSTNumber = addSubscriptionDTO.GSTNumber,
                    LicenceNumber = addSubscriptionDTO.LicenceNumber,
                    UserId = UserId,
                    FinancialYearStart = fyStart,
                    FinancialYearEnd = fyEnd

                };

                var newSubscription = new Subscriptions
                {
                    SubscriptionId = Guid.NewGuid(),
                    OrganizationId = newOrganization.OrganizationId,
                    TransactionId = "pay_Lc1xAzXH3HyeuA",
                    ExpiryDate = DateTime.UtcNow.AddYears(1),
                    SubscriptionName = "Base Plan",
                    Amount = 25000
                };
                var result = await _organizationRepository.AddCompanyWithSubscriptionAsync(newOrganization, newSubscription);
                var organizationSubscribeEvent = new OrganizationSubscribedEvent {OrganizationId= newOrganization.OrganizationId,UserId= UserId };
                _producer.Publish(organizationSubscribeEvent);



                Console.WriteLine("notifying accounts");
                Console.WriteLine("\n");
                Console.WriteLine("notifying accounts");
                Console.WriteLine("notifying accounts");
                Console.WriteLine("notifying accounts");
                Console.WriteLine("notifying accounts");
                Console.WriteLine("notifying accounts");
                Console.WriteLine("\n");
                Console.WriteLine("\n");
                Console.WriteLine("\n");
                Console.WriteLine("\n");
                Console.WriteLine("\n");

                await _notifierGrpc.NotifyOrganizationCreated(newOrganization.OrganizationId, UserId);



                if (!result)
                {
                    return new ApiResponseDTO<string>
                    {
                        StatusCode = 500,
                        Message = "Failed to create organization and subscription.",
                        Data = null
                    };
                }

                return new ApiResponseDTO<string>
                {
                    StatusCode = 200,
                    Message = "Organization and subscription created successfully.",
                    Data = null
                };
            }

            catch (Exception ex)
            {
                Log.Error(ex.Message, "Error occurred while creating subscription for user {UserId}", UserId);
                return new ApiResponseDTO<string>
                {
                    StatusCode = 500,
                    Message = $"Error occurred while creating subscription for user {UserId}",
                    Data = null
                };
            }
        }
        public async Task<ApiResponseDTO<int>> GetTotalCountOfOrganization()
        {
            try
            {
                var result = await _organizationRepository.TotalOrganizationCount();
                if (result == 0)
                {
                    return new ApiResponseDTO<int>
                    {
                        StatusCode = 200,
                        Message = "No Organizations Created",


                    };
                }
                return new ApiResponseDTO<int>
                {
                    StatusCode = 200,
                    Message = "Organization Count fetched successfully",
                    Data = result
                };

            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while counting total organizations.");
                return new ApiResponseDTO<int>
                {
                    StatusCode = 500,
                    Message = "Failed to get total organization count."
                };

            }
        }
        public async Task<ApiResponseDTO<Decimal>> TotalSubscriptionReceivedBySpecificDate(DateTime FromDate, DateTime ToDate)
        {
            try
            {


                var result = await _organizationRepository.TotalSubscriptionReceivedBySpecificDate(FromDate, ToDate);
                if (result == 0)
                {
                    return new ApiResponseDTO<decimal>
                    {
                        StatusCode = 200,
                        Message = "No Amount is received this date"
                    };
                }
                return new ApiResponseDTO<decimal>
                {
                    StatusCode = 200,
                    Message = "Total Amount received  fetched successfully",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "\"An error occurred while getting Total Amount organizations.\"");
                return new ApiResponseDTO<decimal>
                {
                    StatusCode = 500,
                    Message = "Failed to get total amount by a specific date"

                };
            }

        }
        public async Task<ApiResponseDTO<Decimal>> TotalSubscriptionReceivedByCurrentYear()
        {
            try
            {


                var result = await _organizationRepository.TotalSubscriptionReceivedByCurrentYear();
                if (result == 0)
                {
                    return new ApiResponseDTO<decimal>
                    {
                        StatusCode = 200,
                        Message = "No Amount is received this year"
                    };
                }
                return new ApiResponseDTO<decimal>
                {
                    StatusCode = 200,
                    Message = "Total Amount received  fetched successfully",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while getting Total Amount organizations.");
                return new ApiResponseDTO<decimal>
                {
                    StatusCode = 500,
                    Message = "Failed to get total amount by this year"


                };
            }
        }
        public async Task<ApiResponseDTO<Decimal>> TotalSubscriptionReceivedByCurrentMonth()
        {
            try
            {


                var result = await _organizationRepository.TotalSubscriptionReceivedByCurrentMonth();
                if (result == 0)
                {
                    return new ApiResponseDTO<decimal>
                    {
                        StatusCode = 200,
                        Message = "No Amount is received this month"
                    };
                }
                return new ApiResponseDTO<decimal>
                {
                    StatusCode = 200,
                    Message = "Total Amount received  fetched succussfully of this month",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while getting Total Amount organizations.");
                return new ApiResponseDTO<decimal>
                {
                    StatusCode = 500,
                    Message = "Failed to get total amount by this month"


                };
            }
        }
        public async Task<ApiResponseDTO<object>> GetOrganizationAccountStatusSummaryAsync()
        {
            try
            {
                var result = await _organizationRepository.GetOrganizationAccountStatusSummaryAsync();
                return new ApiResponseDTO<object>
                {
                    StatusCode = 200,
                    Message = "Summery fetched successfully ",
                    Data = result
                };

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to get summery of organizations");
                return new ApiResponseDTO<object>
                {
                    StatusCode = 500,
                    Message = "Failed to get summery of organizations",

                };
            }

        }
        public async Task<ApiResponseDTO<List<GetAllOrganizatinsDTO>>> GetAllOrganizationWithSubscription()
        {
            try
            {
                var organization = await _organizationRepository.GetAllOrganizationWithSubscription();
                if (organization.Count == 0)
                {
                    return new ApiResponseDTO<List<GetAllOrganizatinsDTO>>
                    {
                        StatusCode = 200,
                        Message = "No Organization Details"

                    };
                }
                var data = _mapper.Map<List<GetAllOrganizatinsDTO>>(organization);
                return new ApiResponseDTO<List<GetAllOrganizatinsDTO>>
                {
                    StatusCode = 200,
                    Message = "fetched successfully",
                    Data = data
                };

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to get organizations details");
                return new ApiResponseDTO<List<GetAllOrganizatinsDTO>>

                {
                    StatusCode = 500,
                    Message = "Failed to get organizations details"
                };
            }


        }
        public async Task<ApiResponseDTO<bool>> BlockOrganization(Guid organizationId)
        {
            try
            {
                var softdelete = await _organizationRepository.BlockOrganization(organizationId);
                if (softdelete == true)
                {
                    return new ApiResponseDTO<bool>
                    {
                        StatusCode = 200,
                        Message = "Organization blocked successfully "
                    };
                }
                return new ApiResponseDTO<bool>
                {
                    StatusCode = 404,
                    Message = "No Organization for this id"
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Organization block");
                return new ApiResponseDTO<bool>
                {
                    StatusCode = 500,
                    Message = "Error in Organization block"
                };
            }

        }



        //public Task SubscribeUserToOrganizationAsync(Guid userId, Guid organizationId)
        //{
        //    // Save subscription to DB (optional, if needed)

        //    var eventMessage = new OrganizationSubscribedEvent
        //    {
        //        UserId = userId,
        //        OrganizationId = organizationId
        //    };

        //    _producer.Publish(eventMessage);
        //    return Task.CompletedTask;
        //}
    }


}
