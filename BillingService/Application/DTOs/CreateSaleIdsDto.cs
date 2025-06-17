using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateSaleIdsDto
    {
        public Guid InvoiceId {  get; set; }
        public Guid SaleId { get; set; }
        public Guid UserId { get; set; }
        public Guid OrganisationId { get; set; }
    }
}
