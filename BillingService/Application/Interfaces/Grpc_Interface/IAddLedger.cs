using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Application.DTOs;
using Common.ResponseDto;

namespace Application.Interfaces.Grpc_Interface
{
    public  interface IAddLedger
    {

        Task<ResponseDto<string>> AddDebtor(CreateCustomerDto customerData, Guid orgId,Guid userId);
    }
}
