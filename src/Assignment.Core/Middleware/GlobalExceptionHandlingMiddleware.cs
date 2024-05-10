using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Assignment.Api.Middleware
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }
        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            string message;
            string customcode;
          

            switch (exception)
            {
                case ApplicationException appEx:
                    customcode = "R1001";
                    message = appEx.Message;
                    break;
                case KeyNotFoundException keyNotFoundEx:
                    customcode = "R1002";
                    message = keyNotFoundEx.Message;   
                    break;
                case NotImplementedException notImplementedEx:
                    customcode = "R1003";
                    message = notImplementedEx.Message;
                    break;
                case UnauthorizedAccessException unauthorizedEx:
                    customcode = "R1004";
                    message = unauthorizedEx.Message;
                    break;
                case SqlTypeException sqlTypeEx:
                    customcode = "R1005";
                    message = sqlTypeEx.Message;
                    break;
                case InvalidOperationException ex:
                    customcode = "R1006";
                    message = ex.Message;
                    break;
                case ArgumentException argumentEx:
                    customcode = "R1007";
                    message = argumentEx.Message;
                    break;
                default:
                    customcode = "R1000";
                    message = exception.Message;
                    break;
            }

            var exceptionResult = JsonSerializer.Serialize(new
            {
                code = customcode,
                error = message,
            });

            //context.Response.StatusCode = (int)status; // Set the status code for the response
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(exceptionResult);
        }

    }
}
