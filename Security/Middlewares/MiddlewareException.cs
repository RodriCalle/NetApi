using System;
using System.Net;

namespace DefaultProject.Security.Middlewares
{
    public class MiddlewareException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public object Errores { get; set; }

        public MiddlewareException(HttpStatusCode statusCode, object errores)
        {
            StatusCode = statusCode;
            Errores = errores;
        }

    }
}
