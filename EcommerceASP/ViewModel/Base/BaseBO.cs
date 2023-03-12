using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceASP.ViewModel.Base
{
    public class BaseBO
    {
    }
    public class ResponseAPI
    {
        public string Message { get; set; }
        public bool Status { get; set; }
        public int Code { get; set; }
        public int? RowCount { get; set; }
        public object Data { get; set; }


        public ResponseAPI() { }

        public ResponseAPI(string message, bool status, int code, int rowCount, object obj)
        {
            this.Message = message;
            this.Status = status;
            this.Code = code;
            this.RowCount = rowCount;
            this.Data = obj;
        }

        public static ResponseAPI GetFailedResponse(string message)
        {
            return new ResponseAPI()
            {
                Message = message,
                Status = false,
                Code = 0,
                RowCount = 0,
                Data = null
            };
        }
        public static ResponseAPI GetSuccessResponse(string message, object obj)
        {
            return new ResponseAPI()
            {
                Message = message,
                Status = true,
                Code = 1,
                RowCount = 0,
                Data = obj
            };
        }
    }
}