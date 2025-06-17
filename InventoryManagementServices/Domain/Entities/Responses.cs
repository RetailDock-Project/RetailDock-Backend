using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Responses<T>
    {
        public string Message {  get; set; }
        public int StatusCode {  get; set; }
        public  T Data { get; set; }
    }
}
