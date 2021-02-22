using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FieldLevel
{
    public class UnhandledExceptionMiddleware
    {
        private readonly ILogger logger;
        private readonly RequestDelegate next;

        public UnhandledExceptionMiddleware(ILogger<UnhandledExceptionMiddleware> logger, RequestDelegate next)
        {
            this.logger = logger;
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                logger.LogError(exception,
                    $"Request {context.Request?.Method}: {context.Request?.Path.Value} failed");
            }
        }
    }

}

