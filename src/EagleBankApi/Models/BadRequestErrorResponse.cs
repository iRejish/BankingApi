namespace EagleBankApi.Models;

public class BadRequestErrorResponse : ErrorResponse
{
    public List<ErrorDetail> Details { get; set; } = new List<ErrorDetail>();

    public class ErrorDetail
    {
        public string Field { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
    }
}
