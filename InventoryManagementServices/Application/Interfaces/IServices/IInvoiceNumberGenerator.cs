using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IServices
{
    public interface IInvoiceNumberGenerator
    {
        Task<string> GenerateInvoiceNumber(string lastInvoiceNumber, string invoiceType);
    }
}
