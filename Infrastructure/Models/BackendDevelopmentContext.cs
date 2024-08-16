using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models;

public partial class BackendDevelopmentContext : DbContext
{
    public BackendDevelopmentContext()
    {
    }

    public BackendDevelopmentContext(DbContextOptions<BackendDevelopmentContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ArInternalMetadatum> ArInternalMetadata { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<ModelPage> ModelPages { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<SchemaMigration> SchemaMigrations { get; set; }

    public virtual DbSet<Sidebar> Sidebars { get; set; }

    public virtual DbSet<SmartContract> SmartContracts { get; set; }

    public virtual DbSet<Step> Steps { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=127.0.0.1;Database=backend_development;Username=postgres;Password=password");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pgcrypto");

        modelBuilder.Entity<ArInternalMetadatum>(entity =>
        {
            entity.HasKey(e => e.Key).HasName("ar_internal_metadata_pkey");

            entity.ToTable("ar_internal_metadata");

            entity.Property(e => e.Key)
                .HasColumnType("character varying")
                .HasColumnName("key");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp(6) without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp(6) without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.Value)
                .HasColumnType("character varying")
                .HasColumnName("value");
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("documents_pkey");

            entity.ToTable("documents");

            entity.HasIndex(e => e.ModelPageId, "index_documents_on_model_page_id");

            entity.HasIndex(e => e.StepId, "index_documents_on_step_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Address)
                .HasColumnType("character varying")
                .HasColumnName("address");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp(6) without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DocumentNumber)
                .HasColumnType("character varying")
                .HasColumnName("document_number");
            entity.Property(e => e.ModelPageId).HasColumnName("model_page_id");
            entity.Property(e => e.StepId).HasColumnName("step_id");
            entity.Property(e => e.Title)
                .HasColumnType("character varying")
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp(6) without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.ModelPage).WithMany(p => p.Documents)
                .HasForeignKey(d => d.ModelPageId)
                .HasConstraintName("fk_rails_2f161c4848");

            entity.HasOne(d => d.Step).WithMany(p => p.Documents)
                .HasForeignKey(d => d.StepId)
                .HasConstraintName("fk_rails_b7ead67060");
        });

        modelBuilder.Entity<ModelPage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("model_pages_pkey");

            entity.ToTable("model_pages");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.Code)
                .HasColumnType("character varying")
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp(6) without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp(6) without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("permissions_pkey");

            entity.ToTable("permissions");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.Code)
                .HasColumnType("character varying")
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp(6) without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.Title)
                .HasColumnType("character varying")
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp(6) without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.Code)
                .HasColumnType("character varying")
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp(6) without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.Title)
                .HasColumnType("character varying")
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp(6) without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("role_permissions_pkey");

            entity.ToTable("role_permissions");

            entity.HasIndex(e => e.PermissionId, "index_role_permissions_on_permission_id");

            entity.HasIndex(e => e.RoleId, "index_role_permissions_on_role_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp(6) without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp(6) without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .HasConstraintName("fk_rails_439e640a3f");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("fk_rails_60126080bd");
        });

        modelBuilder.Entity<SchemaMigration>(entity =>
        {
            entity.HasKey(e => e.Version).HasName("schema_migrations_pkey");

            entity.ToTable("schema_migrations");

            entity.Property(e => e.Version)
                .HasColumnType("character varying")
                .HasColumnName("version");
        });

        modelBuilder.Entity<Sidebar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sidebars_pkey");

            entity.ToTable("sidebars");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp(6) without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Icon)
                .HasColumnType("character varying")
                .HasColumnName("icon");
            entity.Property(e => e.Order).HasColumnName("order");
            entity.Property(e => e.Path)
                .HasColumnType("character varying")
                .HasColumnName("path");
            entity.Property(e => e.SidebarId).HasColumnName("sidebar_id");
            entity.Property(e => e.Title)
                .HasColumnType("character varying")
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp(6) without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<SmartContract>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("smart_contracts_pkey");

            entity.ToTable("smart_contracts");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp(6) without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.Source).HasColumnName("source");
            entity.Property(e => e.Type)
                .HasColumnType("character varying")
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp(6) without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.Version)
                .HasColumnType("character varying")
                .HasColumnName("version");
        });

        modelBuilder.Entity<Step>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("steps_pkey");

            entity.ToTable("steps");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Code)
                .HasColumnType("character varying")
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp(6) without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Title)
                .HasColumnType("character varying")
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp(6) without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "index_users_on_email").IsUnique();

            entity.HasIndex(e => e.Jti, "index_users_on_jti").IsUnique();

            entity.HasIndex(e => e.ResetPasswordToken, "index_users_on_reset_password_token").IsUnique();

            entity.HasIndex(e => e.RoleId, "index_users_on_role_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp(6) without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasDefaultValueSql("''::character varying")
                .HasColumnType("character varying")
                .HasColumnName("email");
            entity.Property(e => e.EncryptedPassword)
                .HasDefaultValueSql("''::character varying")
                .HasColumnType("character varying")
                .HasColumnName("encrypted_password");
            entity.Property(e => e.FirstName)
                .HasColumnType("character varying")
                .HasColumnName("first_name");
            entity.Property(e => e.Jti)
                .HasColumnType("character varying")
                .HasColumnName("jti");
            entity.Property(e => e.LastName)
                .HasColumnType("character varying")
                .HasColumnName("last_name");
            entity.Property(e => e.RememberCreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("remember_created_at");
            entity.Property(e => e.ResetPasswordSentAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("reset_password_sent_at");
            entity.Property(e => e.ResetPasswordToken)
                .HasColumnType("character varying")
                .HasColumnName("reset_password_token");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp(6) without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("fk_rails_642f17018b");
        });
        modelBuilder.HasSequence("document_number_seq");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
