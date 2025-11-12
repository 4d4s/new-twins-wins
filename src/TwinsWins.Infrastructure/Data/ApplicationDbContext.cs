using Microsoft.EntityFrameworkCore;
using TwinsWins.Core.Entities;

namespace TwinsWins.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<GameParticipant> GameParticipants => Set<GameParticipant>();
    public DbSet<GameMove> GameMoves => Set<GameMove>();
    public DbSet<ImageSet> ImageSets => Set<ImageSet>();
    public DbSet<ImagePair> ImagePairs => Set<ImagePair>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<CouponUsage> CouponUsages => Set<CouponUsage>();
    public DbSet<AffiliateLink> AffiliateLinks => Set<AffiliateLink>();
    public DbSet<AffiliatePayout> AffiliatePayouts => Set<AffiliatePayout>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<GameSession> GameSessions => Set<GameSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.WalletAddress).IsUnique();
            entity.HasIndex(e => e.AffiliateCode).IsUnique();
            entity.HasIndex(e => e.SkillRating);
            
            entity.HasOne(e => e.ReferredBy)
                .WithMany()
                .HasForeignKey(e => e.ReferredByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Game configuration
        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.Status, e.CreatedAt });
            entity.HasIndex(e => e.SmartContractAddress);
            
            entity.Property(e => e.StakeAmount)
                .HasPrecision(18, 8);
        });

        // GameParticipant configuration
        modelBuilder.Entity<GameParticipant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.GameId);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => new { e.GameId, e.UserId }).IsUnique();
            
            entity.Property(e => e.PayoutAmount)
                .HasPrecision(18, 8);
            
            entity.HasOne(e => e.Game)
                .WithMany(g => g.Participants)
                .HasForeignKey(e => e.GameId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.GameParticipants)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // GameMove configuration
        modelBuilder.Entity<GameMove>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.GameId, e.UserId });
            entity.HasIndex(e => e.ServerValidatedAt);
            
            entity.HasOne(e => e.Game)
                .WithMany(g => g.Moves)
                .HasForeignKey(e => e.GameId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ImageSet configuration
        modelBuilder.Entity<ImageSet>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        // ImagePair configuration
        modelBuilder.Entity<ImagePair>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.SetId);
            
            entity.HasOne(e => e.Set)
                .WithMany(s => s.ImagePairs)
                .HasForeignKey(e => e.SetId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Transaction configuration
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.WalletAddress, e.InitiatedAt });
            entity.HasIndex(e => e.GameId);
            entity.HasIndex(e => e.Status);
            
            entity.Property(e => e.Amount)
                .HasPrecision(18, 8);
            
            entity.HasOne(e => e.Game)
                .WithMany(g => g.Transactions)
                .HasForeignKey(e => e.GameId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Coupon configuration
        modelBuilder.Entity<Coupon>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => new { e.IsActive, e.ExpiresAt });
            
            entity.Property(e => e.Value)
                .HasPrecision(18, 8);
        });

        // CouponUsage configuration
        modelBuilder.Entity<CouponUsage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.CouponId, e.UserId, e.GameId }).IsUnique();
            
            entity.Property(e => e.DiscountAmount)
                .HasPrecision(18, 8);
            
            entity.HasOne(e => e.Coupon)
                .WithMany(c => c.Usages)
                .HasForeignKey(e => e.CouponId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // AffiliateLink configuration
        modelBuilder.Entity<AffiliateLink>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ReferrerUserId);
            entity.HasIndex(e => e.ReferredUserId).IsUnique();
            
            entity.Property(e => e.TotalEarnings)
                .HasPrecision(18, 8);
            
            entity.HasOne(e => e.Referrer)
                .WithMany(u => u.ReferralsGiven)
                .HasForeignKey(e => e.ReferrerUserId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.Referred)
                .WithOne(u => u.ReferralReceived)
                .HasForeignKey<AffiliateLink>(e => e.ReferredUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // AffiliatePayout configuration
        modelBuilder.Entity<AffiliatePayout>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Amount)
                .HasPrecision(18, 8);
            
            entity.HasOne(e => e.Link)
                .WithMany(l => l.Payouts)
                .HasForeignKey(e => e.LinkId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // AuditLog configuration
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.CreatedAt });
            entity.HasIndex(e => new { e.Level, e.CreatedAt });
        });

        // GameSession configuration
        modelBuilder.Entity<GameSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.GameId, e.IsActive });
        });
    }
}
