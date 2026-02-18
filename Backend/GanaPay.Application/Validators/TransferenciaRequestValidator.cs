using FluentValidation;
using GanaPay.Application.DTOs.Transacciones;

namespace GanaPay.Application.Validators;

public class TransferenciaRequestValidator : AbstractValidator<TransferenciaRequestDTO>
{
    public TransferenciaRequestValidator()
    {
        RuleFor(x => x.CuentaOrigenId)
            .GreaterThan(0).WithMessage("La cuenta de origen es requerida");

        RuleFor(x => x.CuentaDestinoId)
            .GreaterThan(0).WithMessage("La cuenta de destino es requerida")
            .NotEqual(x => x.CuentaOrigenId)
            .WithMessage("La cuenta destino debe ser diferente a la cuenta origen");

        RuleFor(x => x.Monto)
            .GreaterThan(0).WithMessage("El monto debe ser mayor a cero")
            .LessThanOrEqualTo(100000)
            .WithMessage("El monto no puede exceder Bs. 100,000 por transferencia");

        RuleFor(x => x.Concepto)
            .NotEmpty().WithMessage("El concepto es requerido")
            .MaximumLength(200).WithMessage("El concepto no puede exceder 200 caracteres");
    }
}