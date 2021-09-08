using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FunAPI.Middlewares
{
    public class PaymentMiddleware
    {
        private readonly RequestDelegate _next;

        public PaymentMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (DateTime.Now > new DateTime(2021, 9, 1, 0, 0, 0))
            {
                await context.Response.WriteAsync("Free version of backend expired. Payment awaited.");
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }
}