namespace SchoolProj.Models.DTO
{
    public class Response
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public int Status { get; set; }
        public object? ResponseData { get; set; }
        public object? Error { get; set; }
    }
}
