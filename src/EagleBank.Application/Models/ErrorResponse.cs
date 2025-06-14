namespace EagleBank.Application.Models;

public class ErrorResponse
{
    public string Message { get; set; }
}

public class BadRequestErrorResponse : ErrorResponse
{
    public List<ErrorDetail> Details { get; set; } = new();

    public class ErrorDetail
    {
        public string Field { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
    }
}
