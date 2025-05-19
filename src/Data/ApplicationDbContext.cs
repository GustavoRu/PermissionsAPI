using BackendApi.Permissions.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<PermissionModel> Permissions { get; set; } = null!;
        public DbSet<PermissionTypeModel> PermissionTypes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la relación Permission - PermissionType
            modelBuilder.Entity<PermissionModel>()
                .HasOne(p => p.PermissionType)
                .WithMany(pt => pt.Permissions)
                .HasForeignKey(p => p.PermissionTypeId)
                .OnDelete(DeleteBehavior.Restrict); // Evita el borrado en cascada

            // Configuración de índices
            modelBuilder.Entity<PermissionModel>()
                .HasIndex(p => new { p.EmployeeName, p.EmployeeSurname });

            modelBuilder.Entity<PermissionTypeModel>()
                .HasIndex(pt => pt.Description)
                .IsUnique(); // Asegura que no haya tipos de permiso duplicados

            // Datos semilla para tipos de permiso
            modelBuilder.Entity<PermissionTypeModel>().HasData(
                new PermissionTypeModel { Id = 1, Description = "Enfermedad" },
                new PermissionTypeModel { Id = 2, Description = "Diligencias" },
                new PermissionTypeModel { Id = 3, Description = "Vacaciones" }
            );
        }
    }
}
