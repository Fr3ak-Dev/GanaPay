using GanaPay.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GanaPay.Infrastructure.Data.Configurations;

public class CuentaConfiguration : IEntityTypeConfiguration<Cuenta>
{
    public void Configure(EntityTypeBuilder<Cuenta> builder)
    {
        builder.ToTable("Cuentas");
        
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.NumeroCuenta)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(c => c.Saldo)
            .HasColumnType("decimal(18,2)");
        
        builder.Property(c => c.TipoMoneda)
            .IsRequired()
            .HasConversion<int>();
        
        // Índice único
        builder.HasIndex(c => c.NumeroCuenta)
            .IsUnique();
        
        // Relación con Usuario (ya configurada en UsuarioConfiguration)
    }
}