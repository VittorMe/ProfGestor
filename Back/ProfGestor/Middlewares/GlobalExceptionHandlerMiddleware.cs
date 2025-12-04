using System.Net;
using System.Text.Json;
using ProfGestor.Exceptions;
using ProfGestor.Services;

namespace ProfGestor.Middlewares;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ILogService? logService = null)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado: {Message}", ex.Message);
            
            // Registrar log no banco de dados se o serviço estiver disponível
            if (logService != null)
            {
                try
                {
                    var usuario = context.User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                    var endpoint = context.Request.Path;
                    var metodoHttp = context.Request.Method;
                    var ipAddress = context.Connection.RemoteIpAddress?.ToString();
                    var userAgent = context.Request.Headers["User-Agent"].ToString();

                    await logService.RegistrarLogAsync(
                        "Error",
                        $"Erro não tratado: {ex.Message}",
                        ex,
                        usuario,
                        endpoint,
                        metodoHttp,
                        ipAddress,
                        userAgent
                    );
                }
                catch
                {
                    // Se falhar ao registrar log, não quebrar o fluxo
                }
            }

            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var result = string.Empty;

        switch (exception)
        {
            case NotFoundException notFoundException:
                code = HttpStatusCode.NotFound;
                result = JsonSerializer.Serialize(new { error = notFoundException.Message });
                break;
            case BusinessException businessException:
                code = HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(new { error = businessException.Message });
                break;
            case UnauthorizedAccessException:
                code = HttpStatusCode.Unauthorized;
                result = JsonSerializer.Serialize(new { error = "Acesso não autorizado" });
                break;
            default:
                result = JsonSerializer.Serialize(new { error = "Ocorreu um erro interno no servidor" });
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        return context.Response.WriteAsync(result);
    }
}

