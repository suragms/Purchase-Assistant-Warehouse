namespace PurchaseAssistant.Contracts.Responses
{
    public class ApiResponse<T>
    {
        public T? Data { get; set; }
        public object? Meta { get; set; }
        
        public ApiResponse(T data, object? meta = null)
        {
            Data = data;
            Meta = meta;
        }
    }
}
