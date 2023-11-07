using Microsoft.EntityFrameworkCore;

namespace DataAccess.Models;

public partial class PetShop2023DbContext : DbContext
{
    public PetShop2023DbContext()
    {
    }

    public PetShop2023DbContext(DbContextOptions<PetShop2023DbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Pet> Pets { get; set; }

    public virtual DbSet<PetGroup> PetGroups { get; set; }

    public virtual DbSet<PetShopMember> PetShopMembers { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder.UseSqlServer("Server=(local);uid=sa;pwd=12345;database=PetShop2023DB;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pet>(entity =>
        {
            entity.HasKey(e => e.PetId).HasName("PK__Pet__48E53862C1EEF9AE");

            entity.ToTable("Pet");

            entity.Property(e => e.PetId).ValueGeneratedNever();
            entity.Property(e => e.ImportDate).HasColumnType("datetime");
            entity.Property(e => e.PetDescription).HasMaxLength(220);
            entity.Property(e => e.PetGroupId).HasMaxLength(20);
            entity.Property(e => e.PetName).HasMaxLength(200);

            entity.HasOne(d => d.PetGroup).WithMany(p => p.Pets)
                .HasForeignKey(d => d.PetGroupId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Pet__PetGroupId__29572725");
        });

        modelBuilder.Entity<PetGroup>(entity =>
        {
            entity.HasKey(e => e.PetGroupId).HasName("PK__PetGroup__2C661F98F4FA386A");

            entity.ToTable("PetGroup");

            entity.Property(e => e.PetGroupId).HasMaxLength(20);
            entity.Property(e => e.GroupDescription).HasMaxLength(150);
            entity.Property(e => e.OriginalSource).HasMaxLength(60);
            entity.Property(e => e.PetGroupName).HasMaxLength(80);
        });

        modelBuilder.Entity<PetShopMember>(entity =>
        {
            entity.HasKey(e => e.MemberId).HasName("PK__PetShopM__0CF04B384AEA5F48");

            entity.ToTable("PetShopMember");

            entity.HasIndex(e => e.EmailAddress, "UQ__PetShopM__49A14740142FAB5A").IsUnique();

            entity.Property(e => e.MemberId)
                .HasMaxLength(20)
                .HasColumnName("MemberID");
            entity.Property(e => e.EmailAddress).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(80);
            entity.Property(e => e.MemberPassword).HasMaxLength(80);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
