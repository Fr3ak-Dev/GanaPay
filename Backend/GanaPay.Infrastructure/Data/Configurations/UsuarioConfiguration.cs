using GanaPay.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GanaPay.Infrastructure.Data.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        // Tabla
        builder.ToTable("Usuarios");
        
        // Primary Key
        builder.HasKey(u => u.Id);
        
        // Propiedades
        builder.Property(u => u.NombreCompleto)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(u => u.PasswordHash)
            .IsRequired();
        
        builder.Property(u => u.NumeroDocumento)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.Property(u => u.Telefono)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.Property(u => u.Rol)
            .IsRequired()
            .HasConversion<int>(); // Guarda el enum como int en la DB
        
        // Índices únicos
        builder.HasIndex(u => u.Email)
            .IsUnique();
        
        builder.HasIndex(u => u.NumeroDocumento)
            .IsUnique();
        
        // Relaciones
        builder.HasMany(u => u.Cuentas)
            .WithOne(c => c.Usuario)
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict); // No eliminar usuario si tiene cuentas
    }
}