using Microsoft.EntityFrameworkCore;
using Audimedic_Backend.Enums;

namespace Audimedic_Backend.Data
{
    public class AudimedicDbContext : DbContext
    {
        public AudimedicDbContext(DbContextOptions<AudimedicDbContext> options)
            : base(options) { }

        // Security
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Rol> Roles => Set<Rol>();
        public DbSet<ProveedorExterno> ProveedoresExternos => Set<ProveedorExterno>();

        // Users
        public DbSet<Entidad> Entidades => Set<Entidad>();
        public DbSet<Medico> Medicos => Set<Medico>();
        public DbSet<HistoriaClinica> HistoriasClinicas => Set<HistoriaClinica>();
        public DbSet<ProcedimientoHistoria> ProcedimientosHistoria => Set<ProcedimientoHistoria>();

        // Contratos
        public DbSet<MedicoEntidad> MedicosEntidades => Set<MedicoEntidad>();
        public DbSet<Contrato> Contratos => Set<Contrato>();
        public DbSet<TarifaContrato> TarifasContrato => Set<TarifaContrato>();

        // Catálogo
        public DbSet<Procedimiento> Procedimientos => Set<Procedimiento>();
        public DbSet<TarifaSOAT> TarifasSOAT => Set<TarifaSOAT>();
        public DbSet<UVB> UVB => Set<UVB>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ======== Esquemas ========
            // Security
            modelBuilder.Entity<Usuario>().ToTable("Usuarios", "security");
            modelBuilder.Entity<Rol>().ToTable("Roles", "security");
            modelBuilder.Entity<ProveedorExterno>().ToTable("ProveedoresExternos", "security");

            // Users
            modelBuilder.Entity<Entidad>().ToTable("Entidades", "users");
            modelBuilder.Entity<Medico>().ToTable("Medicos", "users");
            modelBuilder.Entity<HistoriaClinica>().ToTable("HistoriasClinicas", "users");
            modelBuilder.Entity<ProcedimientoHistoria>().ToTable("ProcedimientosHistoria", "users");

            // Contratos
            modelBuilder.Entity<MedicoEntidad>().ToTable("MedicosEntidades", "contratos");
            modelBuilder.Entity<Contrato>().ToTable("Contratos", "contratos");
            modelBuilder.Entity<TarifaContrato>().ToTable("TarifasContrato", "contratos");

            // Catálogo
            modelBuilder.Entity<Procedimiento>().ToTable("Procedimientos", "catalogo");
            modelBuilder.Entity<TarifaSOAT>().ToTable("TarifasSOAT", "catalogo");
            modelBuilder.Entity<UVB>().ToTable("UVB", "catalogo");

            // ======== Relaciones ========
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RolId);

            modelBuilder.Entity<ProveedorExterno>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.ProveedoresExternos)
                .HasForeignKey(p => p.UsuarioId);

            modelBuilder.Entity<MedicoEntidad>()
                .HasOne(me => me.Medico)
                .WithMany(m => m.Entidades)
                .HasForeignKey(me => me.MedicoId);

            modelBuilder.Entity<MedicoEntidad>()
                .HasOne(me => me.Entidad)
                .WithMany(e => e.Medicos)
                .HasForeignKey(me => me.EntidadId);

            modelBuilder.Entity<Contrato>()
                .HasOne(c => c.MedicoEntidad)
                .WithMany(me => me.Contratos)
                .HasForeignKey(c => c.MedicoEntidadId);

            modelBuilder.Entity<Contrato>()
                .Property(c => c.PorcentajeAjuste)
                .HasColumnType("decimal(18,4)");

            modelBuilder.Entity<TarifaContrato>()
                .HasOne(tc => tc.Contrato)
                .WithMany(c => c.Tarifas)
                .HasForeignKey(tc => tc.ContratoId);

            modelBuilder.Entity<TarifaContrato>()
                .HasOne(tc => tc.Procedimiento)
                .WithMany(p => p.TarifasContrato)
                .HasForeignKey(tc => tc.ProcedimientoId);

            modelBuilder.Entity<TarifaContrato>()
            .Property(trfc => trfc.FactorPorcentaje)
            .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<TarifaSOAT>()
                .HasOne(ts => ts.Procedimiento)
                .WithMany(p => p.TarifasSOAT)
                .HasForeignKey(ts => ts.ProcedimientoId);

            modelBuilder.Entity<TarifaSOAT>()
            .Property(t => t.Valor)
            .HasColumnType("decimal(18,2)");


            modelBuilder.Entity<HistoriaClinica>()
                .HasOne(h => h.Medico)
                .WithMany(m => m.Historias)
                .HasForeignKey(h => h.MedicoId);



            modelBuilder.Entity<HistoriaClinica>()
            .HasOne(h => h.Entidad)
            .WithMany(e => e.Historias)
            .HasForeignKey(h => h.EntidadId)
            .OnDelete(DeleteBehavior.SetNull);






            modelBuilder.Entity<Procedimiento>()
                .HasIndex(p => p.CodigoCUPS)
                .IsUnique();

            modelBuilder.Entity<ProcedimientoHistoria>()
                .HasOne(ph => ph.HistoriaClinica)
                .WithMany(h => h.Procedimientos)
                .HasForeignKey(ph => ph.HistoriaClinicaId);

            modelBuilder.Entity<ProcedimientoHistoria>()
                .HasOne(ph => ph.Procedimiento)
                .WithMany()
                .HasForeignKey(ph => ph.ProcedimientoId);

            modelBuilder.Entity<ProcedimientoHistoria>()
            .Property(p => p.ValorCalculado)
            .HasColumnType("decimal(18,2)");



            // ======== Enums como texto ========
            modelBuilder.Entity<Entidad>()
                .Property(e => e.TipoEntidad)
                .HasConversion<string>();

            modelBuilder.Entity<Contrato>()
                .Property(c => c.ManualTarifario)
                .HasConversion<string>();

            modelBuilder.Entity<HistoriaClinica>()
                .Property(h => h.Estado)
                .HasConversion<string>();

            modelBuilder.Entity<ProcedimientoHistoria>()
                .Property(ph => ph.ViaQuirurgica)
                .HasConversion<string>();
        }
    }
}
