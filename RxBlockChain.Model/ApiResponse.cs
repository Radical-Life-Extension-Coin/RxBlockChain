namespace RxBlockChain.Model
{
    public class ApiResponse
    {
        public string ApiVersion { get; set; }

        public string referenceId { get; set; }

        public string code { get; set; }

        public string Message { get; set; }

        public object data { get; set; }

        public ApiError error { get; set; }

        public ApiResponse()
        {
            ApiVersion = "v1";
        }
    }
    public class ApiError
    {
        public string Message { get; set; }
    }
}
