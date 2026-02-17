using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using GanaPay.Application.DTOs.Auth;
using GanaPay.Application.Interfaces;
using GanaPay.Application.Settings;
using GanaPay.Core.Entities;
using GanaPay.Core.Enums;
using GanaPay.Core.Interfaces.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GanaPay.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper _mapper;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IMapper mapper,
        IOptions<JwtSettings> jwtSettings)
    {
        _usuarioRepository = usuarioRepository;
        _mapper = mapper;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequestDTO dto)
    {
        try
        {
            // Verificar si el email ya existe
            if (await _usuarioRepository.ExistsEmailAsync(dto.Email))
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "El email ya está registrado"
                };
            }

            // Verificar si el CI ya existe
            if (await _usuarioRepository.ExistsNumeroDocumentoAsync(dto.NumeroDocumento))
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "El número de documento ya está registrado"
                };
            }

            // Mapear DTO a entidad
            var usuario = _mapper.Map<Usuario>(dto);

            // Configurar propiedades adicionales
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            usuario.Rol = RolUsuario.Cliente;
            usuario.Activo = true;
            usuario.FechaRegistro = DateTime.UtcNow;

            // Guardar en BD
            await _usuarioRepository.AddAsync(usuario);

            // Generar token JWT
            var token = GenerateJwtToken(usuario);

            // Mapear usuario a DTO
            var usuarioDTO = _mapper.Map<UsuarioDTO>(usuario);

            return new AuthResult
            {
                Success = true,
                Message = "Usuario registrado exitosamente",
                Data = new LoginResponseDTO
                {
                    Token = token,
                    Usuario = usuarioDTO
                }
            };
        }
        catch (Exception ex)
        {
            return new AuthResult
            {
                Success = false,
                Message = $"Error al registrar usuario: {ex.Message}"
            };
        }
    }

    public async Task<AuthResult> LoginAsync(LoginRequestDTO dto)
    {
        try
        {
            // Buscar usuario por email
            var usuario = await _usuarioRepository.GetByEmailAsync(dto.Email);

            if (usuario == null)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "Credenciales inválidas"
                };
            }

            // Verificar password con BCrypt
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "Credenciales inválidas"
                };
            }

            // Verificar que el usuario esté activo
            if (!usuario.Activo)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "Usuario inactivo. Contacte al administrador"
                };
            }

            // Generar token JWT
            var token = GenerateJwtToken(usuario);

            // Mapear usuario a DTO
            var usuarioDTO = _mapper.Map<UsuarioDTO>(usuario);

            return new AuthResult
            {
                Success = true,
                Message = "Login exitoso",
                Data = new LoginResponseDTO
                {
                    Token = token,
                    Usuario = usuarioDTO
                }
            };
        }
        catch (Exception ex)
        {
            return new AuthResult
            {
                Success = false,
                Message = $"Error al iniciar sesión: {ex.Message}"
            };
        }
    }

    private string GenerateJwtToken(Usuario usuario)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Name, usuario.NombreCompleto),
            new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
            new Claim("NumeroDocumento", usuario.NumeroDocumento)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}