using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Toucan.Data.Model;

namespace Toucan.Data
{
    public class ToucanContext : DbContext
    {
        public virtual DbSet<Content> Content { get; set; }
        public virtual DbSet<ContentType> ContentType { get; set; }
        public virtual DbSet<Provider> Provider { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserProvider> UserProvider { get; set; }
        public virtual DbSet<UserProviderLocal> LocalProvider { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }

        public ToucanContext() : base()
        {

        }
        public ToucanContext(DbContextOptions<ToucanContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Content>(entity =>
            {
                entity.HasIndex(e => e.ContentTypeId)
                    .HasName("IX_Content_ContentTypeId");

                entity.HasIndex(e => e.UserId)
                    .HasName("IX_Content_UserId");

                entity.Property(e => e.ContentId).HasDefaultValueSql("newid()");

                entity.Property(e => e.Brief).HasColumnType("varchar(512)");

                entity.Property(e => e.ContentData).IsRequired();

                entity.Property(e => e.ContentTypeId)
                    .IsRequired()
                    .HasColumnType("varchar(16)");

                entity.Property(e => e.Enabled).HasDefaultValueSql("1");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnType("varchar(128)");

                entity.HasOne(d => d.ContentType)
                    .WithMany(p => p.Content)
                    .HasForeignKey(d => d.ContentTypeId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Content_ContentType");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Content)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Content_User");
            });

            modelBuilder.Entity<ContentType>(entity =>
            {
                entity.Property(e => e.ContentTypeId).HasColumnType("varchar(16)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("varchar(512)");

                entity.Property(e => e.Enabled).HasDefaultValueSql("1");

                entity.Property(e => e.Name).HasColumnType("varchar(64)");
            });

            modelBuilder.Entity<Provider>(entity =>
            {
                entity.Property(e => e.ProviderId).HasColumnType("varchar(64)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("varchar(512)");

                entity.Property(e => e.Enabled).HasDefaultValueSql("1");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(128)");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.RoleId)
                    .HasColumnType("varchar(16)");

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Enabled).HasDefaultValueSql("0");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(64)");
                
                entity.HasOne(e => e.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(o => o.CreatedBy)
                    .IsRequired();

            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Enabled).HasDefaultValueSql("1");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Verified).HasDefaultValueSql("0");

            });

            modelBuilder.Entity<UserProvider>(entity =>
            {
                entity.HasKey(e => new { e.ProviderId, e.UserId })
                    .HasName("PK_UserProvider");

                entity.HasIndex(e => e.UserId)
                    .HasName("IX_UserProvider_UserId");

                entity.Property(e => e.ProviderId)
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.ExternalId)
                    .HasColumnType("varchar(64)");

                entity.HasOne(d => d.Provider)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.ProviderId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_UserProvider_Provider");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Providers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_UserProvider_User");
                
                 entity.HasDiscriminator<string>("UserProviderType")
                    .HasValue<UserProvider>("External")
                    .HasValue<UserProviderLocal>("Local");
            });

            modelBuilder.Entity<UserProviderLocal>(entity => {

                entity.Property(e => e.PasswordSalt)
                    .HasColumnType("varchar(128)");
                
                entity.Property(e => e.PasswordHash)
                    .HasColumnType("varchar(256)");
                    
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.RoleId, e.UserId })
                    .HasName("PK_UserRole");

                entity.HasIndex(e => e.UserId)
                    .HasName("IX_UserRole_UserId");

                entity.Property(e => e.RoleId).HasColumnType("varchar(16)");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_UserRole_Role");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Roles)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_UserRole_User");
            });
        }
    }
}