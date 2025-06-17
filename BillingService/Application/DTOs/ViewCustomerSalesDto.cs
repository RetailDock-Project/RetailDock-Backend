using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public  class ViewCustomerSalesDto
    {
  public Guid CustomerId { get; set; }  
        public string CustomerName { get; set; }
  public string ContactNumber { get; set; }
  public string? Email { get; set; }
  public string Place { get; set; }
 public string? GstNumber { get; set; }
public List<SalesResponseDto> Sales { get; set; }
}
}
