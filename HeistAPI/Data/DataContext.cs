using HeistAPI.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace HeistAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Member> Members { get; set; }
        public DbSet<MemberSkill> MemberSkills { get; set; }
        public DbSet<Heist> Heists { get; set; }
        public DbSet<HeistSkill> HeistSkills { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MemberSkill>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Level).HasDefaultValue("*");
                entity.Property(e => e.Level).HasMaxLength(10);
                entity.Property(e => e.MemberId).IsRequired(); 
                entity.HasOne<Member>() 
                      .WithMany()
                      .HasForeignKey(e => e.MemberId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Sex).IsRequired();
                entity.Property(e => e.Email).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasMany(e => e.Skills)
                      .WithOne()
                      .HasForeignKey(s => s.MemberId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.MainSkill).IsRequired(false);

                entity.Property(e => e.Status).IsRequired();
            });

            modelBuilder.Entity<HeistSkill>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Level).IsRequired();
                entity.Property(e => e.Members).IsRequired();
                entity.Property(e => e.HeistId).IsRequired();
                entity.HasOne<Heist>()
                      .WithMany(h => h.Skills)
                      .HasForeignKey(e => e.HeistId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Heist>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Location).IsRequired();
                entity.Property(e => e.StartTime).IsRequired();
                entity.Property(e => e.EndTime).IsRequired();
                entity.HasMany(e => e.Skills)
                      .WithOne()
                      .HasForeignKey(s => s.HeistId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
