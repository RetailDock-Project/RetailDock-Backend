﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.ApiResponse
{
    public class ApiResponseDTO<T>
    {
        public int StatusCode {  get; set; }
        public string Message {  get; set; }
        public T Data { get; set; }

    }
}
