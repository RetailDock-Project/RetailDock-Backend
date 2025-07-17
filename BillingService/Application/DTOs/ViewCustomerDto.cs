using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public  class ViewCustomerDto
    {
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string? ContactNumber { get; set; }
        public string? Email { get; set; }
        public string Place { get; set; }
        public bool creditCustomer { get; set; }
        public string GstNumber { get; set; }
        public Guid LedgerId { get; set; }

    }
}
