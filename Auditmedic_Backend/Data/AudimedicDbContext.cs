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
        

        public DbSet<HistoriaCompartida> HistoriasCompartidas => Set<HistoriaCompartida>();
        public DbSet<ArchivoHistoriaClinica> ArchivosHistoriasClinicas => Set<ArchivoHistoriaClinica>(); 
        public DbSet<HistoriaClinicaMedico> HistoriasClinicasMedico => Set<HistoriaClinicaMedico>();



        public DbSet<ProcedimientoHistoria> ProcedimientosHistoria => Set<ProcedimientoHistoria>();

        // Contratos
        public DbSet<MedicoEntidad> MedicosEntidades => Set<MedicoEntidad>();
        public DbSet<Contrato> Contratos => Set<Contrato>();
        public DbSet<TarifaContrato> TarifasContrato => Set<TarifaContrato>();

        // Catálogo
        public DbSet<Procedimiento> Procedimientos => Set<Procedimiento>();
        public DbSet<TarifaSOAT> TarifasSOAT => Set<TarifaSOAT>();
        public DbSet<UVB> UVB => Set<UVB>();
        public DbSet<Factura> Facturas => Set<Factura>();
        public DbSet<FacturaLinea> FacturasLineas => Set<FacturaLinea>();
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
         
           

            modelBuilder.Entity<HistoriaCompartida>().ToTable("HistoriasCompartidas", "users");
            modelBuilder.Entity<ArchivoHistoriaClinica>().ToTable("ArchivosHistoriasClinicas", "users");
            modelBuilder.Entity<HistoriaClinicaMedico>().ToTable("HistoriasClinicasMedico", "users");

            modelBuilder.Entity<ProcedimientoHistoria>().ToTable("ProcedimientosHistoria", "users");

            // Contratos
            modelBuilder.Entity<MedicoEntidad>().ToTable("MedicosEntidades", "contratos");
            modelBuilder.Entity<Contrato>().ToTable("Contratos", "contratos");
            modelBuilder.Entity<TarifaContrato>().ToTable("TarifasContrato", "contratos");

            // Catálogo
            modelBuilder.Entity<Procedimiento>().ToTable("Procedimientos", "catalogo");
            modelBuilder.Entity<TarifaSOAT>().ToTable("TarifasSOAT", "catalogo");
            modelBuilder.Entity<UVB>().ToTable("UVB", "catalogo");


            // Tablas y esquemas
            modelBuilder.Entity<Factura>().ToTable("Facturas", "users");
            modelBuilder.Entity<FacturaLinea>().ToTable("FacturasLineas", "users");


            // Índice único: una historia compartida por Entidad + Numero
            modelBuilder.Entity<HistoriaCompartida>()
                .HasIndex(h => new { h.EntidadId, h.NumeroHistoria })
                .IsUnique();

            // ======== Relaciones ========

            

            modelBuilder.Entity<HistoriaCompartida>()
            .HasOne(h => h.Entidad)
            .WithMany(e => e.HistoriasCompartidas)
            .HasForeignKey(h => h.EntidadId)
            .OnDelete(DeleteBehavior.Restrict);





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

           

            modelBuilder.Entity<TarifaContrato>()
                .HasOne(tc => tc.Contrato)
                .WithMany(c => c.Tarifas)
                .HasForeignKey(tc => tc.ContratoId);

            modelBuilder.Entity<TarifaContrato>()
                .HasOne(tc => tc.Procedimiento)
                .WithMany(p => p.TarifasContrato)
                .HasForeignKey(tc => tc.ProcedimientoId);


            modelBuilder.Entity<TarifaSOAT>()
                .HasOne(ts => ts.Procedimiento)
                .WithMany(p => p.TarifasSOAT)
                .HasForeignKey(ts => ts.ProcedimientoId);
            
            


            modelBuilder.Entity<ArchivoHistoriaClinica>()
            .HasOne(a => a.HistoriaCompartida)
            .WithMany(h => h.Archivos)
            .HasForeignKey(a => a.HistoriaCompartidaId)
            .OnDelete(DeleteBehavior.Cascade);



            modelBuilder.Entity<ArchivoHistoriaClinica>()
            .Property(a => a.TipoArchivo)
            .HasConversion<string>();




            modelBuilder.Entity<Procedimiento>()
                .HasIndex(p => p.CodigoCUPS)
                .IsUnique();

             

            modelBuilder.Entity<ProcedimientoHistoria>()
                .HasOne(ph => ph.HistoriaClinicaMedico )
                .WithMany(h => h.Procedimientos)
                .HasForeignKey(ph => ph.HistoriaClinicaMedicoId );

            modelBuilder.Entity<ProcedimientoHistoria>()
                .HasOne(ph => ph.Procedimiento)
                .WithMany()
                .HasForeignKey(ph => ph.ProcedimientoId);

          
             

            modelBuilder.Entity<Factura>()
            .HasOne(f => f.HistoriaClinicaMedico)
            .WithMany(hm => hm.Facturas)
            .HasForeignKey(f => f.HistoriaClinicaMedicoId)
            .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<FacturaLinea>()
            .HasOne(l => l.Factura)
            .WithMany(f => f.Lineas)
            .HasForeignKey(l => l.FacturaId)
            .OnDelete(DeleteBehavior.Cascade);



            modelBuilder.Entity<FacturaLinea>()
                .HasOne(l => l.ProcedimientoHistoria)
                .WithMany() // o navegación inversa si la tienes
                .HasForeignKey(l => l.ProcedimientoHistoriaId)
                .OnDelete(DeleteBehavior.Restrict);




            modelBuilder.Entity<HistoriaClinicaMedico>()
            .HasOne(hm => hm.HistoriaCompartida)
            .WithMany(hc => hc.HistoriasMedico)
            .HasForeignKey(hm => hm.HistoriaCompartidaId)
            .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<HistoriaClinicaMedico>()
            .HasOne(hm => hm.Medico)
            .WithMany(m => m.Historias)
            .HasForeignKey(hm => hm.MedicoId)
            .OnDelete(DeleteBehavior.Restrict);


 
             


            modelBuilder.Entity<ProcedimientoHistoria>()
            .HasOne(ph => ph.HistoriaClinicaMedico)
            .WithMany(hm => hm.Procedimientos)
            .HasForeignKey(ph => ph.HistoriaClinicaMedicoId)
            .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<ArchivoHistoriaClinica>()
            .Property(a => a.TipoArchivo)
            .HasConversion<string>();


            #region Enums como texto

            // ======== Enums como texto ========
            modelBuilder.Entity<Entidad>()
                .Property(e => e.TipoEntidad)
                .HasConversion<string>();

            modelBuilder.Entity<Contrato>()
                .Property(c => c.ManualTarifario)
                .HasConversion<string>();

          

            modelBuilder.Entity<ProcedimientoHistoria>()
                .Property(ph => ph.ViaQuirurgica)
                .HasConversion<string>();

            // Enums como texto
            modelBuilder.Entity<Factura>()
                .Property(f => f.Estado)
                .HasConversion<string>();
        #endregion

            #region  precision de decimales


            // Precisión decimales
            modelBuilder.Entity<Factura>()
                .Property(f => f.Total)
                .HasPrecision(18, 2);

            modelBuilder.Entity<FacturaLinea>()
                .Property(l => l.ValorUnitario)
                .HasPrecision(18, 2);

            modelBuilder.Entity<FacturaLinea>()
                .Property(l => l.PorcentajeAplicado)
                .HasPrecision(18, 4);

            modelBuilder.Entity<FacturaLinea>()
                .Property(l => l.Subtotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ProcedimientoHistoria>()
          .Property(p => p.ValorCalculado)
          .HasColumnType("decimal(18,2)");


            modelBuilder.Entity<TarifaContrato>()
            .Property(trfc => trfc.FactorPorcentaje)
            .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Contrato>()
               .Property(c => c.PorcentajeAjuste)
               .HasColumnType("decimal(18,4)");

            modelBuilder.Entity<TarifaSOAT>()
          .Property(t => t.Valor)
          .HasColumnType("decimal(18,2)");


            modelBuilder.Entity<TarifaContrato>()
            .Property(t => t.ValorFijo)
            .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ProcedimientoHistoria>()
            .Property(p => p.UVB)
            .HasColumnType("decimal(18,2)");

            #endregion;
        }
    }
}
