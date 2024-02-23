using StockExchangeSystem.Domain;
using System.Net;
using System.Security.Authentication;
using System.Text.Json;

namespace StockExchangeSystem.API.Middleware
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate next;

        public CustomExceptionMiddleware(RequestDelegate _next)
        {
            next = _next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            var statusCode = HttpStatusCode.InternalServerError; // 500

            switch (exception)
            {
                case InvalidTradeDataException:
                    statusCode = HttpStatusCode.BadRequest; // 400
                    break;
                case StockNotFoundException:
                    statusCode = HttpStatusCode.NotFound; // 404
                    break;
                case AuthenticationException:
                    statusCode = HttpStatusCode.Unauthorized; // 401
                    break;
                case AccessDeniedException:
                    statusCode = HttpStatusCode.Forbidden; // 403
                    break;
                case ResourceLimitExceededException:
                    statusCode = HttpStatusCode.TooManyRequests; // 429
                    break;
            }

            response.StatusCode = (int)statusCode;

            var errorResponse = JsonSerializer.Serialize(new { error = exception.Message });
            await response.WriteAsync(errorResponse);
        }
    }
}
