using System;
using System.Collections.Generic;
using System.Text;

namespace YouZack.Helpers
{
    public class BizException:ApplicationException
    {
        public BizException() : base() { }
        
        public BizException(string? message) : base(message) { }

        public BizException(string? message, Exception? innerException) : base(message,innerException) { }

        public int Code { get; set; }
        public string StrCode { get; set; }
        public object Data { get; set; }
    }
}
