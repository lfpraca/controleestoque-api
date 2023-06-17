using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using keevotec.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace keevotec.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            await HandleNotFoundExceptionAsync(context, ex);
        }
        catch (ValidationException ex)
        {
            await HandleValidationExceptionAsync(context, ex);
        }
	catch (DbUpdateException)
	{
            await HandleDbUpdateExceptionAsync(context);
	}
        catch (Exception)
        {
            await HandleExceptionAsync(context);
        }
    }

    private async Task HandleNotFoundExceptionAsync(HttpContext context, NotFoundException exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status404NotFound;

        var result = JsonSerializer.Serialize(new { error = "Conteúdo não encontrado.", Message = exception.Message });
        await context.Response.WriteAsync(result);
    }

    private async Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        var result = JsonSerializer.Serialize(new { error = "Erro de validação.", Message = exception.Message});
        await context.Response.WriteAsync(result);
    }

    private async Task HandleDbUpdateExceptionAsync(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var result = JsonSerializer.Serialize(new { error = "Ocorreu um erro ao modificar o banco de dados."});
        await context.Response.WriteAsync(result);
    }

    private async Task HandleExceptionAsync(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var result = JsonSerializer.Serialize(new { error = "Ocorreu um erro na execução do programa."});
        await context.Response.WriteAsync(result);
    }
}
