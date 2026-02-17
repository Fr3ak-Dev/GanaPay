using FluentValidation;
using GanaPay.Application.DTOs.Auth;
using GanaPay.Core.Interfaces.Repositories;

namespace GanaPay.Application.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequestDTO>
{
    private readonly IUsuarioRepository _usuarioRepository;

    public RegisterRequestValidator(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;

        // ==================== NOMBRE COMPLETO ====================
        RuleFor(x => x.NombreCompleto)
            .NotEmpty().WithMessage("El nombre completo es requerido")
            .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres")
            .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$").WithMessage("El nombre solo puede contener letras y espacios");

        // ==================== EMAIL ====================
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("El formato del email no es válido")
            .MaximumLength(100).WithMessage("El email no puede exceder 100 caracteres")
            .MustAsync(async (email, cancellation) => !await _usuarioRepository.ExistsEmailAsync(email))
            .WithMessage("El email ya está registrado");

        // ==================== PASSWORD ====================
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres")
            .Matches(@"[A-Z]").WithMessage("La contraseña debe contener al menos una letra mayúscula")
            .Matches(@"[a-z]").WithMessage("La contraseña debe contener al menos una letra minúscula")
            .Matches(@"[0-9]").WithMessage("La contraseña debe contener al menos un número");

        // ==================== CONFIRM PASSWORD ====================
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Debe confirmar la contraseña")
            .Equal(x => x.Password).WithMessage("Las contraseñas no coinciden");

        // ==================== NUMERO DOCUMENTO ====================
        RuleFor(x => x.NumeroDocumento)
            .NotEmpty().WithMessage("El número de documento es requerido")
            .Matches(@"^\d{7,8}$").WithMessage("El número de documento debe tener 7 u 8 dígitos numéricos")
            .MustAsync(async (numeroDocumento, cancellation) => !await _usuarioRepository.ExistsNumeroDocumentoAsync(numeroDocumento))
            .WithMessage("El número de documento ya está registrado");

        // ==================== TELEFONO ====================
        RuleFor(x => x.Telefono)
            .NotEmpty().WithMessage("El teléfono es requerido")
            .Matches(@"^\d{8}$").WithMessage("El teléfono debe tener exactamente 8 dígitos");
    }
}