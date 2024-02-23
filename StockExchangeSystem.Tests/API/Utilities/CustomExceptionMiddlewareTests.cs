using Microsoft.AspNetCore.Http;
using StockExchangeSystem.API.Middleware;
using StockExchangeSystem.Domain;
using System.Net;
using System.Security.Authentication;

namespace StockExchangeSystem.Tests.API.Utilities
{
    public class CustomExceptionMiddlewareTests
    {
        private static CustomExceptionMiddleware CreateMiddlewareWithException(Type exceptionType)
        {
            return new CustomExceptionMiddleware((innerHttpContext) =>
            {
                Exception exceptionToThrow = exceptionType switch
                {
                    _ when exceptionType == typeof(InvalidTradeDataException) => new InvalidTradeDataException("Error message"),
                    _ when exceptionType == typeof(StockNotFoundException) => new StockNotFoundException("Error message"),
                    _ when exceptionType == typeof(AuthenticationException) => new AuthenticationException("Error message"),
                    _ when exceptionType == typeof(AccessDeniedException) => new AccessDeniedException("Error message"),
                    _ when exceptionType == typeof(ResourceLimitExceededException) => new ResourceLimitExceededException("Error message"),
                    _ => new Exception("Error message"),
                };
                throw exceptionToThrow;
            });
        }

        [Theory]
        [InlineData(typeof(InvalidTradeDataException), HttpStatusCode.BadRequest)]
        [InlineData(typeof(StockNotFoundException), HttpStatusCode.NotFound)]
        [InlineData(typeof(AuthenticationException), HttpStatusCode.Unauthorized)]
        [InlineData(typeof(AccessDeniedException), HttpStatusCode.Forbidden)]
        [InlineData(typeof(ResourceLimitExceededException), HttpStatusCode.TooManyRequests)]
        public async Task Invoke_ExceptionThrown_SetsExpectedStatusCode(Type exceptionType, HttpStatusCode expectedStatusCode)
        {
            // Arrange
            var middleware = CreateMiddlewareWithException(exceptionType);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(context.Response.Body);
            var responseBody = await reader.ReadToEndAsync();

            Assert.Equal(expectedStatusCode, (HttpStatusCode)context.Response.StatusCode);
            Assert.Contains("Error message", responseBody);
        }
    }
}
