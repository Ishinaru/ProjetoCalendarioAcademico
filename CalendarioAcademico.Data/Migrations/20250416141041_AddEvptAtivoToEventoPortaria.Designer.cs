﻿// <auto-generated />
using System;
using CalendarioAcademico.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CalendarioAcademico.Data.Migrations
{
    [DbContext(typeof(CalendarioAcademicoContext))]
    [Migration("20250416141041_AddEvptAtivoToEventoPortaria")]
    partial class AddEvptAtivoToEventoPortaria
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CalendarioAcademico.Domain.Models.CAD_Calendario", b =>
                {
                    b.Property<int>("CAD_CD_Calendario")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CAD_CD_Calendario"));

                    b.Property<int?>("CAD_Ano")
                        .HasColumnType("int");

                    b.Property<int?>("CAD_CD_Evento")
                        .HasColumnType("int");

                    b.Property<int?>("CAD_CD_Usuario")
                        .HasColumnType("int");

                    b.Property<string>("CAD_DS_Observacao")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CAD_DT_DataAtualizacao")
                        .HasColumnType("datetime");

                    b.Property<string>("CAD_NumeroResolucao")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<int>("CAD_Status")
                        .HasColumnType("int");

                    b.HasKey("CAD_CD_Calendario")
                        .HasName("PK__CAD_Cale__4D5927FA14D61171");

                    b.HasIndex(new[] { "CAD_Ano" }, "UQ__CAD_Cale__8448D96D4FFD3E11")
                        .IsUnique()
                        .HasFilter("[CAD_Ano] IS NOT NULL");

                    b.ToTable("CAD_Calendario");
                });

            modelBuilder.Entity("CalendarioAcademico.Domain.Models.EVNT_Evento", b =>
                {
                    b.Property<int>("EVNT_CD_Evento")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EVNT_CD_Evento"));

                    b.Property<bool?>("EVNT_Ativo")
                        .HasColumnType("bit");

                    b.Property<int?>("EVNT_CD_Calendario")
                        .HasColumnType("int");

                    b.Property<int?>("EVNT_CD_Usuario")
                        .HasColumnType("int");

                    b.Property<string>("EVNT_DS_Descricao")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("EVNT_DT_DataAtualizacao")
                        .HasColumnType("datetime");

                    b.Property<DateOnly?>("EVNT_DT_DataFinal")
                        .HasColumnType("date");

                    b.Property<DateOnly?>("EVNT_DT_DataInicio")
                        .HasColumnType("date");

                    b.Property<bool?>("EVNT_Importante")
                        .HasColumnType("bit");

                    b.Property<int?>("EVNT_TipoFeriado")
                        .HasColumnType("int");

                    b.Property<bool?>("EVNT_UescFunciona")
                        .HasColumnType("bit");

                    b.HasKey("EVNT_CD_Evento")
                        .HasName("PK__EVNT_Eve__680DEC4CA71646BA");

                    b.HasIndex("EVNT_CD_Calendario");

                    b.ToTable("EVNT_Evento");
                });

            modelBuilder.Entity("CalendarioAcademico.Domain.Models.EVPT_Evento_Portaria", b =>
                {
                    b.Property<int>("EVPT_CD_Evento")
                        .HasColumnType("int");

                    b.Property<int>("EVPT_CD_Portaria")
                        .HasColumnType("int");

                    b.Property<bool>("EVPT_Ativo")
                        .HasColumnType("bit");

                    b.Property<DateOnly?>("EVPT_DT_DataFinal")
                        .HasColumnType("date");

                    b.Property<DateOnly?>("EVPT_DT_DataInicio")
                        .HasColumnType("date");

                    b.Property<string>("EVPT_Observacao")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EVPT_CD_Evento", "EVPT_CD_Portaria")
                        .HasName("PK__EVPT_Eve__B1CAA9895B5C21B0");

                    b.HasIndex("EVPT_CD_Portaria");

                    b.ToTable("EVPT_Evento_Portaria");
                });

            modelBuilder.Entity("CalendarioAcademico.Domain.Models.PORT_Portaria", b =>
                {
                    b.Property<int>("PORT_CD_Portaria")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PORT_CD_Portaria"));

                    b.Property<int?>("PORT_Ano")
                        .HasColumnType("int");

                    b.Property<bool?>("PORT_Ativo")
                        .HasColumnType("bit");

                    b.Property<int?>("PORT_CD_Usuario")
                        .HasColumnType("int");

                    b.Property<DateTime?>("PORT_DT_DataAtualizacao")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(format(getdate(),'dd/MM/yyyy - HH:mm:ss'))");

                    b.Property<string>("PORT_NumPortaria")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("PORT_Observacao")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PORT_CD_Portaria")
                        .HasName("PK__PORT_Por__D2D1F3F64F83A8A5");

                    b.ToTable("PORT_Portaria");
                });

            modelBuilder.Entity("CalendarioAcademico.Domain.Models.EVNT_Evento", b =>
                {
                    b.HasOne("CalendarioAcademico.Domain.Models.CAD_Calendario", "EVNT_CD_CalendarioNavigation")
                        .WithMany("EVNT_Evento")
                        .HasForeignKey("EVNT_CD_Calendario")
                        .HasConstraintName("FK_Calendario");

                    b.Navigation("EVNT_CD_CalendarioNavigation");
                });

            modelBuilder.Entity("CalendarioAcademico.Domain.Models.EVPT_Evento_Portaria", b =>
                {
                    b.HasOne("CalendarioAcademico.Domain.Models.EVNT_Evento", "EVPT_CD_EventoNavigation")
                        .WithMany("EVPT_Evento_Portaria")
                        .HasForeignKey("EVPT_CD_Evento")
                        .IsRequired()
                        .HasConstraintName("FK_Evento");

                    b.HasOne("CalendarioAcademico.Domain.Models.PORT_Portaria", "EVPT_CD_PortariaNavigation")
                        .WithMany("EVPT_Evento_Portaria")
                        .HasForeignKey("EVPT_CD_Portaria")
                        .IsRequired()
                        .HasConstraintName("FK_Portaria");

                    b.Navigation("EVPT_CD_EventoNavigation");

                    b.Navigation("EVPT_CD_PortariaNavigation");
                });

            modelBuilder.Entity("CalendarioAcademico.Domain.Models.CAD_Calendario", b =>
                {
                    b.Navigation("EVNT_Evento");
                });

            modelBuilder.Entity("CalendarioAcademico.Domain.Models.EVNT_Evento", b =>
                {
                    b.Navigation("EVPT_Evento_Portaria");
                });

            modelBuilder.Entity("CalendarioAcademico.Domain.Models.PORT_Portaria", b =>
                {
                    b.Navigation("EVPT_Evento_Portaria");
                });
#pragma warning restore 612, 618
        }
    }
}
