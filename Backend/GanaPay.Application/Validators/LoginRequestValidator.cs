using FluentValidation;
using GanaPay.Application.DTOs.Auth;

namespace GanaPay.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequestDTO>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("El formato del email no es válido");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida");
    }
}