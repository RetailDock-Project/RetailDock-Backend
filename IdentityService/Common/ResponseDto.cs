﻿namespace Common
{
    public class ResponseDto<T>
    {
        public int StatusCode { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
    }
}
