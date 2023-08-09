using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DefaultProject.Security.Middlewares
{
    public class ManagerMiddleware
    {
        private readonly RequestDelegate requestDelegate;
        private readonly ILogger<ManagerMiddleware> logger;

        public ManagerMiddleware(RequestDelegate requestDelegate, ILogger<ManagerMiddleware> logger)
        {
            this.requestDelegate = requestDelegate;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await requestDelegate(context);
            }
            catch (Exception e)
            {
                await ManagerExceptionAsync(context, e, logger);
            }
        }

        private async Task ManagerExceptionAsync(HttpContext context, Exception e, ILogger<ManagerMiddleware> logger)
        {
            object? errores = null;

            switch (e)
            {
                case MiddlewareException me:
                    logger.LogError(e, "Middleware Exception");
                    errores = me.Errores;
                    break;

                case Exception exception:
                    logger.LogError(exception, "Error de servidor");
                    errores = string.IsNullOrWhiteSpace(exception.Message) ? "Error" : exception.Message;
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            context.Response.ContentType = "application/json";
            var resultados = string.Empty;

            if (errores != null)
            {
                resultados = JsonConvert.SerializeObject(errores);
            }

            await context.Response.WriteAsync(resultados);
        }
    }
}
