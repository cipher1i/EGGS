using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Access.Entities
{
    public partial class EGGSDBContext : DbContext
    {
        public EGGSDBContext()
        {
        }

        public EGGSDBContext(DbContextOptions<EGGSDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Ip> Ip { get; set; }
        public virtual DbSet<Request> Request { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured){}
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ip>(entity =>
            {
                entity.HasKey(e => e.Email)
                    .HasName("IP_PK");

                entity.ToTable("IP");

                entity.HasIndex(e => e.Email)
                    .HasName("UQ__IP__A9D1053412A55751")
                    .IsUnique();

                entity.Property(e => e.Email)
                    .HasMaxLength(345)
                    .IsUnicode(false);

                entity.Property(e => e.Ipv4)
                    .HasColumnName("IPv4")
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Ipv6)
                    .HasColumnName("IPv6")
                    .HasMaxLength(39)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.HasOne(d => d.EmailNavigation)
                    .WithOne(p => p.Ip)
                    .HasPrincipalKey<User>(p => p.Email)
                    .HasForeignKey<Ip>(d => d.Email)
                    .HasConstraintName("IP_FK");
            });

            modelBuilder.Entity<Request>(entity =>
            {
                entity.HasKey(e => e.Email)
                    .HasName("REQUEST_PK");

                entity.ToTable("REQUEST");

                entity.HasIndex(e => e.Email)
                    .HasName("UQ__REQUEST__A9D1053498C630CB")
                    .IsUnique();

                entity.Property(e => e.Email)
                    .HasMaxLength(345)
                    .IsUnicode(false);

                entity.Property(e => e.Status).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.EmailNavigation)
                    .WithOne(p => p.Request)
                    .HasPrincipalKey<User>(p => p.Email)
                    .HasForeignKey<Request>(d => d.Email)
                    .HasConstraintName("REQUEST_FK");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("USER_PK")
                    .IsClustered(false);

                entity.ToTable("USER");

                entity.HasIndex(e => e.Email)
                    .HasName("UIX_USER_EMAIL")
                    .IsUnique()
                    .IsClustered();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(345)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.LastName)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(128)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
