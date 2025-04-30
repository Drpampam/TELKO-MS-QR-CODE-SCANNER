using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Responses
{
    public class BaseResponse<T>
    {
        public BaseResponse() { }
        public BaseResponse(string message)
        {
            Message = message;
        }

        public BaseResponse(string message, T data, string responseCode)
        {

            Message = message;
            Data = data;
            ResponseCode = responseCode;
        }

        public BaseResponse(string message, string responseCode)
        {

            Message = message;
            ResponseCode = responseCode;
        }

        public T Data { get; set; }
        public string Message { get; set; }
        public string ResponseCode { get; set; }
    }
}
