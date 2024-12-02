using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace Actividades_Semestral.Models;

public partial class GestorActividadesContext : DbContext
{
    public GestorActividadesContext()
    {
    }

    public GestorActividadesContext(DbContextOptions<GestorActividadesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Actividade> Actividades { get; set; }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<EstadoActividade> EstadoActividades { get; set; }

    public virtual DbSet<Inscripcione> Inscripciones { get; set; }

    public virtual DbSet<Notificacione> Notificaciones { get; set; }

    public virtual DbSet<PropuestasActividade> PropuestasActividades { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Subcategoria> Subcategorias { get; set; }

    public virtual DbSet<TipoEstado> TipoEstados { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseMySql("database=gestorActividades;datasource=localhost;user id=root;password=shiro180504", ServerVersion.Parse("8.0.38-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Actividade>(entity =>
        {
            entity.HasKey(e => e.IdActividad).HasName("PRIMARY");

            entity.ToTable("actividades");

            entity.HasIndex(e => e.IdEstado, "id_estado");

            entity.HasIndex(e => e.IdSubcategoria, "id_subcategoria");

            entity.Property(e => e.IdActividad).HasColumnName("id_actividad");
            entity.Property(e => e.Costo)
                .HasPrecision(10, 2)
                .HasColumnName("costo");
            entity.Property(e => e.Descripcion)
                .HasColumnType("text")
                .HasColumnName("descripcion");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.Hora)
                .HasColumnType("time")
                .HasColumnName("hora");
            entity.Property(e => e.IdEstado)
                .HasDefaultValueSql("'1'")
                .HasColumnName("id_estado");
            entity.Property(e => e.IdSubcategoria).HasColumnName("id_subcategoria");
            entity.Property(e => e.ImagenUrl)
                .HasMaxLength(255)
                .HasColumnName("imagen_url");
            entity.Property(e => e.LimiteCupos).HasColumnName("limite_cupos");
            entity.Property(e => e.Lugar)
                .HasMaxLength(100)
                .HasColumnName("lugar");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Requisitos)
                .HasColumnType("text")
                .HasColumnName("requisitos");

            entity.HasOne(d => d.IdEstadoNavigation).WithMany(p => p.Actividades)
                .HasForeignKey(d => d.IdEstado)
                .HasConstraintName("actividades_ibfk_2");

            entity.HasOne(d => d.IdSubcategoriaNavigation).WithMany(p => p.Actividades)
                .HasForeignKey(d => d.IdSubcategoria)
                .HasConstraintName("actividades_ibfk_1");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("PRIMARY");

            entity.ToTable("categorias");

            entity.Property(e => e.IdCategoria).HasColumnName("id_categoria");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<EstadoActividade>(entity =>
        {
            entity.HasKey(e => e.IdEstado).HasName("PRIMARY");

            entity.ToTable("estado_actividades");

            entity.HasIndex(e => e.IdTipoEstado, "id_tipo_estado");

            entity.HasIndex(e => e.NombreEstado, "nombre_estado").IsUnique();

            entity.Property(e => e.IdEstado).HasColumnName("id_estado");
            entity.Property(e => e.IdTipoEstado).HasColumnName("id_tipo_estado");
            entity.Property(e => e.NombreEstado)
                .HasMaxLength(50)
                .HasColumnName("nombre_estado");

            entity.HasOne(d => d.IdTipoEstadoNavigation).WithMany(p => p.EstadoActividades)
                .HasForeignKey(d => d.IdTipoEstado)
                .HasConstraintName("estado_actividades_ibfk_1");
        });

        modelBuilder.Entity<Inscripcione>(entity =>
        {
            entity.HasKey(e => e.IdInscripcion).HasName("PRIMARY");

            entity.ToTable("inscripciones");

            entity.HasIndex(e => e.IdActividad, "id_actividad");

            entity.HasIndex(e => e.IdEstado, "id_estado");

            entity.HasIndex(e => e.IdUsuario, "id_usuario");

            entity.Property(e => e.IdInscripcion).HasColumnName("id_inscripcion");
            entity.Property(e => e.FechaInscripcion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("fecha_inscripcion");
            entity.Property(e => e.IdActividad).HasColumnName("id_actividad");
            entity.Property(e => e.IdEstado)
                .HasDefaultValueSql("'7'")
                .HasColumnName("id_estado");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

            entity.HasOne(d => d.IdActividadNavigation).WithMany(p => p.Inscripciones)
                .HasForeignKey(d => d.IdActividad)
                .HasConstraintName("inscripciones_ibfk_2");

            entity.HasOne(d => d.IdEstadoNavigation).WithMany(p => p.Inscripciones)
                .HasForeignKey(d => d.IdEstado)
                .HasConstraintName("inscripciones_ibfk_3");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Inscripciones)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("inscripciones_ibfk_1");
        });

        modelBuilder.Entity<Notificacione>(entity =>
        {
            entity.HasKey(e => e.IdNotificacion).HasName("PRIMARY");

            entity.ToTable("notificaciones");

            entity.HasIndex(e => e.IdUsuario, "id_usuario");

            entity.Property(e => e.IdNotificacion).HasColumnName("id_notificacion");
            entity.Property(e => e.FechaNotificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("fecha_notificacion");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Leido)
                .HasDefaultValueSql("'0'")
                .HasColumnName("leido");
            entity.Property(e => e.Mensaje)
                .HasColumnType("text")
                .HasColumnName("mensaje");
            entity.Property(e => e.TipoNotificacion)
                .HasMaxLength(100)
                .HasColumnName("tipo_notificacion");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Notificaciones)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("notificaciones_ibfk_1");
        });

        modelBuilder.Entity<PropuestasActividade>(entity =>
        {
            entity.HasKey(e => e.IdPropuesta).HasName("PRIMARY");

            entity.ToTable("propuestas_actividades");

            entity.HasIndex(e => e.IdEstado, "id_estado");

            entity.HasIndex(e => e.IdSubcategoria, "id_subcategoria");

            entity.HasIndex(e => e.IdUsuario, "id_usuario");

            entity.Property(e => e.IdPropuesta).HasColumnName("id_propuesta");
            entity.Property(e => e.ComentarioRechazo)
                .HasColumnType("text")
                .HasColumnName("comentario_rechazo");
            entity.Property(e => e.Costo)
                .HasPrecision(10, 2)
                .HasColumnName("costo");
            entity.Property(e => e.Descripcion)
                .HasColumnType("text")
                .HasColumnName("descripcion");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.Hora)
                .HasColumnType("time")
                .HasColumnName("hora");
            entity.Property(e => e.IdEstado)
                .HasDefaultValueSql("'9'")
                .HasColumnName("id_estado");
            entity.Property(e => e.IdSubcategoria).HasColumnName("id_subcategoria");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.ImagenUrl)
                .HasMaxLength(255)
                .HasColumnName("imagen_url");
            entity.Property(e => e.LimiteCupos).HasColumnName("limite_cupos");
            entity.Property(e => e.Lugar)
                .HasMaxLength(100)
                .HasColumnName("lugar");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Requisitos)
                .HasColumnType("text")
                .HasColumnName("requisitos");

            

            entity.HasOne(d => d.IdEstadoNavigation).WithMany(p => p.PropuestasActividades)
                .HasForeignKey(d => d.IdEstado)
                .HasConstraintName("propuestas_actividades_ibfk_3");

            entity.HasOne(d => d.IdSubcategoriaNavigation).WithMany(p => p.PropuestasActividades)
                .HasForeignKey(d => d.IdSubcategoria)
                .HasConstraintName("propuestas_actividades_ibfk_2");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.PropuestasActividades)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("propuestas_actividades_ibfk_1");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PRIMARY");

            entity.ToTable("roles");

            entity.HasIndex(e => e.NombreRol, "nombre_rol").IsUnique();

            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.NombreRol)
                .HasMaxLength(50)
                .HasColumnName("nombre_rol");
        });

        modelBuilder.Entity<Subcategoria>(entity =>
        {
            entity.HasKey(e => e.IdSubcategoria).HasName("PRIMARY");

            entity.ToTable("subcategorias");

            entity.HasIndex(e => e.IdCategoria, "id_categoria");

            entity.Property(e => e.IdSubcategoria).HasColumnName("id_subcategoria");
            entity.Property(e => e.IdCategoria).HasColumnName("id_categoria");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Subcategoria)
                .HasForeignKey(d => d.IdCategoria)
                .HasConstraintName("subcategorias_ibfk_1");
        });

        modelBuilder.Entity<TipoEstado>(entity =>
        {
            entity.HasKey(e => e.IdTipoEstado).HasName("PRIMARY");

            entity.ToTable("tipo_estado");

            entity.HasIndex(e => e.NombreTipo, "nombre_tipo").IsUnique();

            entity.Property(e => e.IdTipoEstado).HasColumnName("id_tipo_estado");
            entity.Property(e => e.NombreTipo)
                .HasMaxLength(50)
                .HasColumnName("nombre_tipo");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PRIMARY");

            entity.ToTable("usuarios");

            entity.HasIndex(e => e.Correo, "correo").IsUnique();

            entity.HasIndex(e => e.IdRol, "id_rol");

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .HasColumnName("correo");
            entity.Property(e => e.Facultad)
                .HasMaxLength(100)
                .HasColumnName("facultad");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("usuarios_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
