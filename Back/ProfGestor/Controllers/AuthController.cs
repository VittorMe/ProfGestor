using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using ProfGestor.DTOs;
using ProfGestor.Services;

namespace ProfGestor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IWebHostEnvironment _environment;

    public AuthController(IAuthService authService, IWebHostEnvironment environment)
    {
        _authService = authService;
        _environment = environment;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _authService.LoginAsync(request);
        if (response == null)
            return Unauthorized(new { message = "Email ou senha inválidos" });

        // Configurar cookie HttpOnly para armazenar o token
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, // Não acessível via JavaScript (proteção XSS)
            Secure = !_environment.IsDevelopment(), // Apenas HTTPS em produção
            SameSite = SameSiteMode.Strict, // Proteção CSRF
            Expires = response.ExpiraEm,
            Path = "/"
        };

        // Definir cookie com o token
        Response.Cookies.Append("authToken", response.Token, cookieOptions);

        // Não retornar o token no body (apenas no cookie)
        var responseWithoutToken = new LoginResponse
        {
            ExpiraEm = response.ExpiraEm,
            Professor = response.Professor
            // Token não é incluído aqui por segurança
        };

        return Ok(responseWithoutToken);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var success = await _authService.RegisterAsync(request);
        if (!success)
            return Conflict(new { message = "Email já cadastrado" });

        return Ok(new { message = "Professor cadastrado com sucesso" });
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var professorId = long.Parse(User.FindFirst("ProfessorId")?.Value ?? "0");
        if (professorId == 0)
            return Unauthorized();

        var success = await _authService.ChangePasswordAsync(
            professorId, 
            request.SenhaAtual, 
            request.NovaSenha
        );

        if (!success)
            return BadRequest(new { message = "Senha atual inválida" });

        return Ok(new { message = "Senha alterada com sucesso" });
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        // Remover cookie de autenticação
        Response.Cookies.Delete("authToken", new CookieOptions
        {
            HttpOnly = true,
            Secure = !_environment.IsDevelopment(),
            SameSite = SameSiteMode.Strict,
            Path = "/"
        });

        return Ok(new { message = "Logout realizado com sucesso" });
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        var professorId = User.FindFirst("ProfessorId")?.Value;
        var nome = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        return Ok(new
        {
            Id = professorId,
            Nome = nome,
            Email = email
        });
    }
}

