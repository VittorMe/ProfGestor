using System.ComponentModel.DataAnnotations;

namespace ProfGestor.DTOs;

public class ChangePasswordRequest
{
    [Required(ErrorMessage = "Senha atual é obrigatória")]
    public string SenhaAtual { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nova senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Nova senha deve ter no mínimo 6 caracteres")]
    public string NovaSenha { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
    [Compare("NovaSenha", ErrorMessage = "As senhas não coincidem")]
    public string ConfirmarNovaSenha { get; set; } = string.Empty;
}

