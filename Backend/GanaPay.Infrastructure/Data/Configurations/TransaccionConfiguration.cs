using GanaPay.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GanaPay.Infrastructure.Data.Configurations;

public class TransaccionConfiguration : IEntityTypeConfiguration<Transaccion>
{
    public void Configure(EntityTypeBuilder<Transaccion> builder)
    {
        builder.ToTable("Transacciones");
        
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.TipoTransaccion)
            .IsRequired()
            .HasConversion<int>();
        
        builder.Property(t => t.Monto)
            .HasColumnType("decimal(18,2)");
        
        builder.Property(t => t.Moneda)
            .IsRequired()
            .HasConversion<int>();
        
        builder.Property(t => t.Concepto)
            .HasMaxLength(500);
        
        builder.Property(t => t.Estado)
            .IsRequired()
            .HasConversion<int>();
        
        builder.Property(t => t.ReferenciaExterna)
            .HasMaxLength(100);
        
        builder.Property(t => t.CodigoQR)
            .HasMaxLength(200);
        
        // Relaciones con Cuenta
        builder.HasOne(t => t.CuentaOrigen)
            .WithMany(c => c.TransaccionesOrigen)
            .HasForeignKey(t => t.CuentaOrigenId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(t => t.CuentaDestino)
            .WithMany(c => c.TransaccionesDestino)
            .HasForeignKey(t => t.CuentaDestinoId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Índices para performance
        builder.HasIndex(t => t.FechaHora);
        builder.HasIndex(t => t.Estado);
    }
}