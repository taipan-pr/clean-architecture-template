using Clean.Architecture.Template.Application.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Serilog;
using Serilog.Context;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Clean.Architecture.Template.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public ExceptionMiddleware(RequestDelegate next, ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            await using var request = new MemoryStream();
            await using var response = new MemoryStream();
            var originalBody = context.Response.Body;
            var route = context.GetRouteData();
            if (route.Values.TryGetValue("controller", out var controller))
            {
                LogContext.PushProperty("Controller", controller?.ToString());
            }

            if (route.Values.TryGetValue("action", out var action))
            {
                LogContext.PushProperty("ControllerMethod", action?.ToString());
            }

            try
            {
                var requestJson = await GetRequestBodyAsync(context, request);
                LogContext.PushProperty("JsonRequest", string.IsNullOrWhiteSpace(requestJson) ? "null" : requestJson);
                PushLogProperties(context);
                _logger.Information("Start: [{Method}] {Controller}.{Action}", context.Request.Method, controller,
                    action);

                context.Response.Body = response;
                context.Response.Headers.Add("x-environment", new StringValues(_configuration["Serilog:Properties:Environment"]));
                context.Response.Headers.Add("x-release", new StringValues(_configuration["Serilog:Properties:Environment"]));
                await _next(context);
                stopwatch.Stop();
                response.Seek(0, SeekOrigin.Begin);
                var responseJson = await new StreamReader(context.Response.Body).ReadToEndAsync();
                response.Seek(0, SeekOrigin.Begin);
                await response.CopyToAsync(originalBody);
                if (!string.IsNullOrWhiteSpace(context.Request.ContentType) &&
                    context.Request.ContentType.Contains("multipart/form-data"))
                {
                    _logger
                        .Information("Finished: [{Method}] {Controller}.{Action} - {Elapsed}", context.Request.Method,
                            controller, action, stopwatch.Elapsed);
                }
                else
                {
                    _logger
                        .ForContext("JsonResponse", string.IsNullOrWhiteSpace(responseJson) ? "null" : responseJson)
                        .Information("Finished: [{Method}] {Controller}.{Action} - {Elapsed}", context.Request.Method,
                            controller, action, stopwatch.Elapsed);
                }
            }
            catch (ValidationException ex)
            {
                stopwatch.Stop();
                context.Response.Body = originalBody;
                var exceptionResponse = new
                {
                    message = "Validation failed",
                    details = ex.Errors.Select(e => e.ErrorMessage)
                };
                _logger
                    .Warning(ex, "Finished ValidationException: [{Method}] {Controller}.{Action} - {Elapsed}",
                        context.Request.Method, controller, action, stopwatch.Elapsed);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var json = JsonSerializer.Serialize(exceptionResponse);
                await context.Response.WriteAsync(json);
            }
            catch (ExceptionBase ex)
            {
                stopwatch.Stop();
                context.Response.Body = originalBody;
                var exceptionResponse = new
                {
                    message = ex.Message
                };
                _logger
                    .Warning(ex, "Finished {ExceptionType}: [{Method}] {Controller}.{Action} - {Elapsed}",
                        ex.GetType().Name, context.Request.Method, controller, action, stopwatch.Elapsed);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var json = JsonSerializer.Serialize(exceptionResponse);
                await context.Response.WriteAsync(json);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                context.Response.Body = originalBody;
                var exceptionResponse = new { message = ex.Message };
                _logger
                    .Error(ex, "Finished Exception: [{Method}] {Controller}.{Action} - {Elapsed}",
                        context.Request.Method, controller, action, stopwatch.Elapsed);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                if (_configuration["Serilog:Properties:Environment"] == "Local" ||
                    _configuration["Serilog:Properties:Environment"] == "Development")
                {
                    var json = JsonSerializer.Serialize(exceptionResponse);
                    await context.Response.WriteAsync(json);
                }
            }
        }

        private void PushLogProperties(HttpContext context)
        {
            LogContext.PushProperty("Protocol", context.Request.Protocol);
            LogContext.PushProperty("PathBase", context.Request.PathBase);
            LogContext.PushProperty("Path", context.Request.Path);
            LogContext.PushProperty("Method", context.Request.Method);
            LogContext.PushProperty("ContentType", context.Request.ContentType);
            LogContext.PushProperty("ContentLength", context.Request.ContentLength);
        }

        private async Task<string> GetRequestBodyAsync(HttpContext context, Stream stream)
        {
            if ((!string.IsNullOrWhiteSpace(context.Request.ContentType) &&
                 context.Request.ContentType.Contains("multipart/form-data")) ||
                context.Request.ContentLength is null or 0) return default;
            using var bodyReader = new StreamReader(context.Request.Body);
            var bodyAsText = await bodyReader.ReadToEndAsync();
            var bytesToWrite = Encoding.UTF8.GetBytes(bodyAsText);
            await stream.WriteAsync(bytesToWrite, 0, bytesToWrite.Length);
            stream.Seek(0, SeekOrigin.Begin);
            context.Request.Body = stream;
            return bodyAsText.Replace(" ", string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty);
        }

        private async Task<string> GetResponseBodyAsync(HttpContext context, Stream stream)
        {
            using var bodyReader = new StreamReader(context.Response.Body);
            var bodyAsText = await bodyReader.ReadToEndAsync();
            var obj = JsonSerializer.Deserialize<dynamic>(bodyAsText);
            var bytesToWrite = Encoding.UTF8.GetBytes(bodyAsText);
            await stream.WriteAsync(bytesToWrite, 0, bytesToWrite.Length);
            stream.Seek(0, SeekOrigin.Begin);
            context.Request.Body = stream;

            return bodyAsText;
        }
    }
}
