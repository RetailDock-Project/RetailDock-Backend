using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IServices;

namespace Application.Services
{
    public class InvoiceNumberGenerator:IInvoiceNumberGenerator
    {
        public Task<string> GenerateInvoiceNumber(string lastInvoiceNumber, string invoiceType)
        {
            int newSerialNumber = 1;

            if (!string.IsNullOrEmpty(lastInvoiceNumber))
            {
                var parts = lastInvoiceNumber.Split('-');
                if (parts.Length == 3 && parts[0] == invoiceType && int.TryParse(parts[2], out int lastNumber))
                {
                    newSerialNumber = lastNumber + 1;
                }
            }

            var currentYear = DateTime.Now.Year;
            var newInvoiceNumber = $"{invoiceType}-{currentYear}-{newSerialNumber:D4}";

            return Task.FromResult(newInvoiceNumber);
        }
    }
}
