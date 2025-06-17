using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Common.Mapper
{
    public class MappingProfile :Profile
    {
        public MappingProfile()
        {
            CreateMap<OrganizationDetails, GetAllOrganizatinsDTO>()
                .ForMember(dest => dest.SubscriptionId, opt => opt.MapFrom(src => src.Subscriptions.SubscriptionId))
                .ForMember(dest => dest.SubscriptionName, opt => opt.MapFrom(src => src.Subscriptions.SubscriptionName))
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.Subscriptions.TransactionId))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Subscriptions.Amount))
                .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.Subscriptions.ExpiryDate))
                .ForMember(dest => dest.SubscriptionUpdatedAt, opt => opt.MapFrom(src => src.Subscriptions.UpdatedAt));
        }
    }
}
