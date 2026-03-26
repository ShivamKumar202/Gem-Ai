using System;
using System.ComponentModel;

namespace Gem.COMMON.Enum
{
    public enum StatusCode
    {
        // Success status codes
        [Description("OK")]
        OK = 200,

        [Description("Created")]
        Created = 201,

        // Client error status codes
        [Description("Bad Request")]
        BadRequest = 400,

        [Description("UnAutorised")]
        Unauthorized = 401,

        [Description("Forbidden")]
        Forbidden = 403,

        [Description("NotFound")]
        NotFound = 404,

        // Server error status codes
        [Description("InternalServerError")]
        InternalServerError = 500,

        [Description("NotImplemented")]
        NotImplemented = 501,

        [Description("BadGateway")]
        BadGateway = 502,

        [Description("ServiceUnavailable")]
        ServiceUnavailable = 503,

       [Description("RequestTimeout")]
        RequestTimeout = 408,

        [Description("Operation Canceled")]
        OperationCanceled=499
    }
}
