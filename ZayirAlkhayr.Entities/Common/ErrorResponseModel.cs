using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Common
{
    public class ErrorResponseModel<T>
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public T? Results { get; set; }

        public static ErrorResponseModel<T> Success(Error SuccessError, T? value = default)
        {
            return new ErrorResponseModel<T>
            {
                IsSuccess = true,
                Message = SuccessError.Message,
                Results = value
            };
        }

        public static ErrorResponseModel<T> Failure(Error Error, T? value = default)
        {
            return new ErrorResponseModel<T>
            {
                IsSuccess = false,
                Message = Error.Message,
                Results = value
            };
        }
    }
}
