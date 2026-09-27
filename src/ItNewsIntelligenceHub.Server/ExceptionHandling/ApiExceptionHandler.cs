using System.Net;
using System.Xml;
using ItNewsIntelligenceHub.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ItNewsIntelligenceHub.Server.ExceptionHandling;

public sealed class ApiExceptionHandler(
    ILogger<ApiExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = CreateProblemDetails(
            httpContext,
            exception,
            out var logLevel);

        logger.Log(
            logLevel,
            exception,
            "Request failed. TraceId: {TraceId}, Path: {RequestPath}, StatusCode: {StatusCode}",
            httpContext.TraceIdentifier,
            httpContext.Request.Path,
            problemDetails.Status);

        httpContext.Response.StatusCode =
            problemDetails.Status ?? StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }

    private static ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        Exception exception,
        out LogLevel logLevel)
    {
        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path
        };

        switch (exception)
        {
            case NotFoundException:
                problemDetails.Status = StatusCodes.Status404NotFound;
                problemDetails.Title = "Resource not found";
                problemDetails.Detail = exception.Message;
                logLevel = LogLevel.Information;
                break;

            case ConflictException:
                problemDetails.Status = StatusCodes.Status409Conflict;
                problemDetails.Title = "Request conflicts with the current resource state";
                problemDetails.Detail = exception.Message;
                logLevel = LogLevel.Warning;
                break;

            case HttpRequestException:
                problemDetails.Status = StatusCodes.Status502BadGateway;
                problemDetails.Title = "External feed request failed";
                problemDetails.Detail =
                    "The RSS or Atom feed could not be downloaded.";
                logLevel = LogLevel.Warning;
                break;

            case XmlException:
                problemDetails.Status = StatusCodes.Status502BadGateway;
                problemDetails.Title = "External feed contains invalid XML";
                problemDetails.Detail =
                    "The RSS or Atom feed could not be parsed.";
                logLevel = LogLevel.Warning;
                break;

            case ExternalServiceException:
                problemDetails.Status = StatusCodes.Status502BadGateway;
                problemDetails.Title = "External service request failed";
                problemDetails.Detail = exception.Message;
                logLevel = LogLevel.Warning;
                break;

            default:
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "An unexpected error occurred";
                problemDetails.Detail =
                    "An unexpected server error occurred. Please try again later.";
                logLevel = LogLevel.Error;
                break;
        }

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        return problemDetails;
    }
}