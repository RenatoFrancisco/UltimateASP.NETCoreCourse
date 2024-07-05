using HotelListing.Api.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace HotelListing.Api.Core.Middleware;

public class ExceptionMiddlaware(RequestDelegate next, ILogger<ExceptionMiddlaware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
		try
		{
			await next(context);
		}
		catch (Exception ex)
		{
            logger.LogError(ex, $"Something went wrong while processing {context.Request.Path}");

			await HandleExceptionAsync(context, ex);
		}
    }

    private Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
		var statusCode = HttpStatusCode.InternalServerError;
        var errorDetails = new ErrorDetails
        {
            ErrorType = "Failure",
            ErrorMessage = ex.Message
        };

        switch (ex) 
        { 
            case NotFoundException:
                statusCode = HttpStatusCode.NotFound;
                errorDetails .ErrorType = "Not Found";
                break;
            default:
                break;
        }

        var response = JsonConvert.SerializeObject(errorDetails);
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsync(response);
    }
}

public class ErrorDetails
{
    public string? ErrorType { get; set; }
    public string? ErrorMessage { get; set; }
}
