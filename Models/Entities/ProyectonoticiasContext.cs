using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebNoticias.Models.Entities;

public partial class ProyectonoticiasContext : DbContext
{
    public ProyectonoticiasContext()
    {
    }

    public ProyectonoticiasContext(DbContextOptions<ProyectonoticiasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categorias> Categorias { get; set; }

    public virtual DbSet<Noticias> Noticias { get; set; }

    public virtual DbSet<Rol> Rol { get; set; }

    public virtual DbSet<Usuario> Usuario { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=websitos256.com;database=websitos_noticiasangel;username=websitos_dbnoticiasangel;password=#svK5582w", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.30-mysql"));
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Categorias>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("categorias");

            entity.Property(e => e.Nombre).HasMaxLength(45);
        });

        modelBuilder.Entity<Noticias>(entity =>
        {
            entity.HasKey(e => e.NoticiaId).HasName("PRIMARY");

            entity
                .ToTable("noticias")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.AutorId, "AutorID");

            entity.HasIndex(e => e.IdCategoria, "FK_noticias_1");

            entity.Property(e => e.NoticiaId).HasColumnName("NoticiaID");
            entity.Property(e => e.AutorId).HasColumnName("AutorID");
            entity.Property(e => e.Caption).HasColumnType("text");
            entity.Property(e => e.Contenido).HasColumnType("text");
            entity.Property(e => e.FechaPublicacion).HasColumnType("datetime");
            entity.Property(e => e.Imagen).HasColumnType("text");
            entity.Property(e => e.Titulo).HasMaxLength(100);

            entity.HasOne(d => d.Autor).WithMany(p => p.Noticias)
                .HasForeignKey(d => d.AutorId)
                .HasConstraintName("noticias_ibfk_1");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Noticias)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_noticias_1");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("rol");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Nombre).HasMaxLength(30);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PRIMARY");

            entity.ToTable("usuario");

            entity.HasIndex(e => e.Correo, "Correo").IsUnique();

            entity.HasIndex(e => e.IdRol, "IdRol");

            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");
            entity.Property(e => e.Contraseña).HasMaxLength(128);
            entity.Property(e => e.Correo).HasMaxLength(100);
            entity.Property(e => e.Nombre).HasMaxLength(50);

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuario)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("usuario_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
