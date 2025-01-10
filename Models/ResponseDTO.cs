namespace SchoolProj.Models
{
    public class ResponseDTO<T>
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public int Status { get; set; }
        public T? ResponseData { get; set; }
        public string? Error { get; set; }
    }
}
