using CalendarioAcademico.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CalendarioAcademico.Data.DBContext;

public partial class CalendarioAcademicoContext : DbContext
{
    public CalendarioAcademicoContext()
    {
    }

    public CalendarioAcademicoContext(DbContextOptions<CalendarioAcademicoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CAD_Calendario> CAD_Calendario { get; set; }

    public virtual DbSet<EVNT_Evento> EVNT_Evento { get; set; }

    public virtual DbSet<EVPT_Evento_Portaria> EVPT_Evento_Portaria { get; set; }

    public virtual DbSet<PORT_Portaria> PORT_Portaria { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=localhost;Database=NovoCalendarioAcademico;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CAD_Calendario>(entity =>
        {
            entity.HasKey(e => e.CAD_CD_Calendario).HasName("PK__CAD_Cale__4D5927FA14D61171");
        });

        modelBuilder.Entity<EVNT_Evento>(entity =>
        {
            entity.HasKey(e => e.EVNT_CD_Evento).HasName("PK__EVNT_Eve__680DEC4CA71646BA");

            entity.HasOne(d => d.EVNT_CD_CalendarioNavigation).WithMany(p => p.EVNT_Evento).HasConstraintName("FK_Calendario");
        });

        modelBuilder.Entity<EVPT_Evento_Portaria>(entity =>
        {
            entity.HasKey(e => new { e.EVPT_CD_Evento, e.EVPT_CD_Portaria }).HasName("PK__EVPT_Eve__B1CAA9895B5C21B0");

            entity.HasOne(d => d.EVPT_CD_EventoNavigation).WithMany(p => p.EVPT_Evento_Portaria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Evento");

            entity.HasOne(d => d.EVPT_CD_PortariaNavigation).WithMany(p => p.EVPT_Evento_Portaria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Portaria");
        });

        modelBuilder.Entity<PORT_Portaria>(entity =>
        {
            entity.HasKey(e => e.PORT_CD_Portaria).HasName("PK__PORT_Por__D2D1F3F64F83A8A5");

            entity.Property(e => e.PORT_DT_DataAtualizacao).HasDefaultValueSql("(format(getdate(),'dd/MM/yyyy - HH:mm:ss'))");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
