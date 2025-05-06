using System;
using System.Collections.Generic;
using CalendarioAcademico.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CalendarioAcademico.Domain;

public partial class CalendarioAcademicoContext : DbContext
{
    public CalendarioAcademicoContext()
    {
    }

    public CalendarioAcademicoContext(DbContextOptions<CalendarioAcademicoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CadCalendario> CadCalendarios { get; set; }

    public virtual DbSet<EvntEvento> EvntEventos { get; set; }

    public virtual DbSet<EvptEventoPortarium> EvptEventoPortaria { get; set; }

    public virtual DbSet<PortPortarium> PortPortaria { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=NovoCalendarioAcademico;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CadCalendario>(entity =>
        {
            entity.HasKey(e => e.CadCdCalendario).HasName("PK__CAD_Cale__4D5927FA14D61171");

            entity.Property(e => e.CadDtDataAtualizacao).HasDefaultValueSql("(format(getdate(),'dd/MM/yyyy - HH:mm:ss'))");
        });

        modelBuilder.Entity<EvntEvento>(entity =>
        {
            entity.HasKey(e => e.EvntCdEvento).HasName("PK__EVNT_Eve__680DEC4CA71646BA");

            entity.Property(e => e.EvntDtDataAtualizacao).HasDefaultValueSql("(format(getdate(),'dd/MM/yyyy - HH:mm:ss'))");

            entity.HasOne(d => d.EvntCdCalendarioNavigation).WithMany(p => p.EvntEventos).HasConstraintName("FK_Calendario");
        });

        modelBuilder.Entity<EvptEventoPortarium>(entity =>
        {
            entity.HasKey(e => new { e.EvptCdEvento, e.EvptCdPortaria }).HasName("PK__EVPT_Eve__B1CAA9895B5C21B0");

            entity.HasOne(d => d.EvptCdEventoNavigation).WithMany(p => p.EvptEventoPortaria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Evento");

            entity.HasOne(d => d.EvptCdPortariaNavigation).WithMany(p => p.EvptEventoPortaria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Portaria");
        });

        modelBuilder.Entity<PortPortarium>(entity =>
        {
            entity.HasKey(e => e.PortCdPortaria).HasName("PK__PORT_Por__D2D1F3F64F83A8A5");

            entity.Property(e => e.PortDtDataAtualizacao).HasDefaultValueSql("(format(getdate(),'dd/MM/yyyy - HH:mm:ss'))");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
