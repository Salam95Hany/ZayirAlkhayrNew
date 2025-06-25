using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Common
{
    public class ApiResponseModel<T>
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public T? Results { get; set; }

        public static ApiResponseModel<T> Success(Error SuccessError, T? value = default)
        {
            return new ApiResponseModel<T>
            {
                IsSuccess = true,
                Message = SuccessError.Message,
                Results = value
            };
        }

        public static ApiResponseModel<T> Failure(Error Error)
        {
            return new ApiResponseModel<T>
            {
                IsSuccess = false,
                Message = Error.Message,
                Results = default
            };
        }
    }
}
