using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L_AI.TextGeneration.WebApi
{
    public class ApiResult<T>
    {
        public bool IsSuccessful { get; set; }
        public T Data { get; set; }
        public string ErrorMessage { get; set; }


        public static ApiResult<T> Result(T result)
        {
            return new ApiResult<T>
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Data = result,
            };
        }

        public static ApiResult<T> Error(string errorMessage)
        {
            return new ApiResult<T>
            {
                IsSuccessful = false,
                ErrorMessage = errorMessage,
                Data = default,
            };
        }
    }
}
