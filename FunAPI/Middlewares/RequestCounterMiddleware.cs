using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Services.CommonServices.Abstractions;

namespace FunAPI.Middlewares
{
    public class RequestCounterMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly IRequestCounterService _requestCounterService;

        public RequestCounterMiddleware(RequestDelegate next, IRequestCounterService requestCounterService)
        {
            _next = next;
            _requestCounterService = requestCounterService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _requestCounterService.Notice(context.Request.Path);
            
            string path = context.Request.Path;
            path = path.ToLower();
            if (path.EndsWith(".php") || path.Contains("phpmyadmin"))
            {
                await context.Response
                    .WriteAsync(
                        "Dear friend! " +
                        "Please don't access .php files or phpMyAdmin. " +
                        "We are not running on PHP)"
                    );
                return;
            }

            await _next.Invoke(context);
        }
    }
}