using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Threading.Tasks;
using TeamRanking.API.Middleware;
using Xunit;

namespace TeamRanking.API.Tests.Middleware
{
    public class ExceptionHandlingMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_NoException_CallsNextMiddleware()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
            var middleware = new ExceptionHandlingMiddleware((innerHttpContext) => Task.CompletedTask, loggerMock.Object);

            // Act
            await middleware.InvokeAsync(httpContext);

            // Assert
            Assert.Equal(200, httpContext.Response.StatusCode); 
        }

        [Fact]
        public async Task InvokeAsync_WithException_LogsErrorAndReturns500()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();

            RequestDelegate next = (innerHttpContext) =>
            {
                throw new Exception("Test exception");
            };

            var middleware = new ExceptionHandlingMiddleware(next, loggerMock.Object);

            var responseBody = new MemoryStream();
            httpContext.Response.Body = responseBody;

            // Act
            await middleware.InvokeAsync(httpContext);

            // Assert
            Assert.Equal(500, httpContext.Response.StatusCode);
            Assert.Equal("application/json", httpContext.Response.ContentType);

            responseBody.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(responseBody);
            var responseText = await reader.ReadToEndAsync();

            Assert.Equal("{\"error\": \"An internal error occurred.\"}", responseText);

            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An unhandled exception occurred")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}