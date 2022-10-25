using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace SecurityService.Models
{
    public partial class LmsDBContext : DbContext
    {
        public LmsDBContext()
        {
        }

        public LmsDBContext(DbContextOptions<LmsDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=CTSDOTNET094;Initial Catalog=LmsDB;Persist Security Info=False;User Id = sa; Password = pass@word1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<User>(entity =>
            {
           

                entity.ToTable("users");

                entity.Property(e => e.LmsUserId).HasColumnName("lms_user_id");
                entity.HasKey(e => e.LmsUserId)
               .HasName("pk_id");

                entity.Property(e => e.IsAdmin).HasColumnName("is_Admin");

                entity.Property(e => e.LmsEmailId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("lms_email_id");

                entity.Property(e => e.LmsMobileNumber)
                    .HasColumnType("numeric(10, 0)")
                    .HasColumnName("lms_mobile_number");

                entity.Property(e => e.LmsUserName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("lms_user_name");

                entity.Property(e => e.LmsUserPassword)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("lms_user_password");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
