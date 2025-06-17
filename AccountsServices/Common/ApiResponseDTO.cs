namespace Common
{
    public class ApiResponseDTO<T>
    {
        public int StatusCode {  get; set; }
        public string Message { get; set; }
        public T Data { get; set; }


    }
}
