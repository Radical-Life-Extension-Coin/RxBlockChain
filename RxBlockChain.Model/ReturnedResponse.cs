using RxBlockChain.Model.Enums;

namespace RxBlockChain.Model
{
    public static class ReturnedResponse
    {
        public static ApiResponse ErrorResponse(string message, object data)
        {
            var apiResp = new ApiResponse();
            apiResp.data = data;
            apiResp.Message = Status.Unsuccessful.ToString();
            apiResp.code = 400;
            var error = new ApiError();
            error.Message = message;
            apiResp.error = error;

            return apiResp;
        }

        public static ApiResponse SuccessResponse(string message, object data, string ReferenceId = "")
        {
            var apiResp = new ApiResponse();
            apiResp.data = data;
            apiResp.referenceId = ReferenceId;
            apiResp.Message = Status.Successful.ToString();
            apiResp.code = 200;
            var error = new ApiError();
            error.Message = message;
            apiResp.error = error;

            return apiResp;
        }

    }
    public static class ReturnedResponse<T>
    {
        public static ApiResponse<T> ErrorResponse(string message, T data)
        {
            var apiResp = new ApiResponse<T>();
            apiResp.data = data;
            apiResp.Message = Status.Unsuccessful.ToString();
            apiResp.code = 400;
            var error = new ApiError();
            error.Message = message;
            apiResp.error = error;

            return apiResp;
        }

        public static ApiResponse<T> SuccessResponse(string message, T data, string ReferenceId = "")
        {
            var apiResp = new ApiResponse<T>();
            apiResp.data = data;
            apiResp.referenceId = ReferenceId;
            apiResp.Message = Status.Successful.ToString();
            apiResp.code = 200;
            var error = new ApiError();
            error.Message = message;
            apiResp.error = error;

            return apiResp;
        }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public new T data { get; set; }
    }
}
