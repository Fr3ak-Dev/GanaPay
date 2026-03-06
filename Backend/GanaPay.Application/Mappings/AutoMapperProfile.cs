using AutoMapper;
using GanaPay.Application.DTOs.Auth;
using GanaPay.Application.DTOs.Cuentas;
using GanaPay.Application.DTOs.Transacciones;
using GanaPay.Core.Entities;
using GanaPay.Core.Enums;

namespace GanaPay.Application.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // ==================== MAPEOS DE USUARIO ====================

        // Mapeo simple
        CreateMap<Usuario, UsuarioDTO>();
        // Mapeo con colección anidada
        CreateMap<Usuario, UsuarioConCuentasDTO>()  // Usuario → UsuarioConCuentasDTO (incluye mapeo de cuentas)
            .ForMember(dest => dest.Cuentas, opt => opt.MapFrom(src => src.Cuentas));
        // Mapeo inverso
        CreateMap<RegisterRequestDTO, Usuario>()    // Para crear nuevo usuario
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Se asigna manualmente con BCrypt
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Auto-generado por BD
            .ForMember(dest => dest.Rol, opt => opt.Ignore()) // Se asigna manualmente (siempre Cliente)
            .ForMember(dest => dest.Activo, opt => opt.Ignore()) // Se asigna manualmente (true)
            .ForMember(dest => dest.FechaRegistro, opt => opt.Ignore()) // Se asigna manualmente (DateTime.UtcNow)
            .ForMember(dest => dest.Cuentas, opt => opt.Ignore()); // No se crean cuentas en registro

        // ==================== MAPEOS DE CUENTA ====================

        // Mapeo condicional con navigation properties
        CreateMap<Cuenta, CuentaDTO>()
            .ForMember(dest => dest.NombreUsuario, opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.NombreCompleto : null));

        // ==================== MAPEOS DE TRANSACCION ====================

        // Mapeo de Transaccion con 2 navigation properties
        CreateMap<Transaccion, TransaccionDTO>()
            .ForMember(dest => dest.NumeroCuentaOrigen, opt => opt.MapFrom(src => src.CuentaOrigen != null ? src.CuentaOrigen.NumeroCuenta : null))
            .ForMember(dest => dest.NumeroCuentaDestino, opt => opt.MapFrom(src => src.CuentaDestino != null ? src.CuentaDestino.NumeroCuenta : null));

        CreateMap<TransferenciaRequestDTO, Transaccion>()   // Para crear transacción
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Auto-generado
            .ForMember(dest => dest.TipoTransaccion, opt => opt.Ignore()) // Se asigna manualmente
            .ForMember(dest => dest.Moneda, opt => opt.Ignore()) // Se asigna manualmente
            .ForMember(dest => dest.Estado, opt => opt.Ignore()) // Se asigna manualmente
            .ForMember(dest => dest.FechaHora, opt => opt.Ignore()) // Se asigna manualmente
            .ForMember(dest => dest.CuentaOrigen, opt => opt.Ignore()) // Navigation property
            .ForMember(dest => dest.CuentaDestino, opt => opt.Ignore()) // Navigation property
            .ForMember(dest => dest.ReferenciaExterna, opt => opt.Ignore())
            .ForMember(dest => dest.CodigoQR, opt => opt.Ignore());

        // ==================== NUEVOS MAPEOS DE CUENTA ====================

        CreateMap<Cuenta, ResumenCuentaDTO>()
            .ForMember(dest => dest.NombreUsuario,
                opt => opt.MapFrom(src =>
                    src.Usuario != null ? src.Usuario.NombreCompleto : null))
            .ForMember(dest => dest.TipoMonedaDescripcion,
                opt => opt.MapFrom(src =>
                    src.TipoMoneda == TipoMoneda.Bolivianos ? "Bolivianos (Bs.)" : "Dólares ($)"))
            .ForMember(dest => dest.TotalTransacciones,
                opt => opt.MapFrom(src =>
                    src.TransaccionesOrigen.Count + src.TransaccionesDestino.Count));

        CreateMap<CrearCuentaDTO, Cuenta>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.NumeroCuenta, opt => opt.Ignore())
            .ForMember(dest => dest.Activa, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.Usuario, opt => opt.Ignore())
            .ForMember(dest => dest.TransaccionesOrigen, opt => opt.Ignore())
            .ForMember(dest => dest.TransaccionesDestino, opt => opt.Ignore())
            .ForMember(dest => dest.Saldo,
                opt => opt.MapFrom(src => src.SaldoInicial));
    }
}