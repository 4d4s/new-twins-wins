using Microsoft.EntityFrameworkCore;
using TwinsWins.Core.Entities;

namespace TwinsWins.Infrastructure.Data;

public class ApplicationDbContextUpdated : DbContext
{
    public ApplicationDbContextUpdated(DbContextOptions<ApplicationDbContextUpdated> options)
        : base(options)
    {
    }

    // Use new entity names that match database
    public DbSet<User> Users => Set<User>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<GameParticipant> GameParticipants => Set<GameParticipant>();
    public DbSet<GameMove> GameMoves => Set<GameMove>();
    public DbSet<ImageSet> ImageSets => Set<ImageSet>();
    public DbSet<ImagePair> ImagePairs => Set<ImagePair>();

    // Changed from Transaction to BlockchainTransaction
    public DbSet<BlockchainTransaction> BlockchainTransactions => Set<BlockchainTransaction>();

    public DbSet<Coupon> Coupons => Set<Coupon>();

    // Changed from CouponUsage to CouponRedemption
    public DbSet<CouponRedemption> CouponRedemptions => Set<CouponRedemption>();

    public DbSet<AffiliateLink> AffiliateLinks => Set<AffiliateLink>();
    public DbSet<AffiliatePayout> AffiliatePayouts => Set<AffiliatePayout>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<GameSession> GameSessions => Set<GameSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration (NO internal balance)
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.WalletAddress).IsUnique();
            entity.HasIndex(e => e.AffiliateCode).IsUnique();
            entity.HasIndex(e => e.SkillRating);

            entity.HasOne(e => e.ReferredBy)
                .WithMany()
                .HasForeignKey(e => e.ReferredByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // BlockchainTransactions (renamed from Transaction)
        modelBuilder.Entity<BlockchainTransaction>(entity =>
        {
            entity.ToTable("BlockchainTransactions");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.TransactionHash).IsUnique();
            entity.HasIndex(e => e.FromWallet);
            entity.HasIndex(e => e.ToWallet);
            entity.HasIndex(e => e.GameId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Type);

            entity.Property(e => e.Amount).HasPrecision(18, 8);
        });

        // Game configuration
        modelBuilder.Entity<Game>(entity =>
        {
            entity.ToTable("Games");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.Status, e.CreatedAt });
            entity.HasIndex(e => e.SmartContractAddress);

            entity.Property(e => e.StakeAmount).HasPrecision(18, 8);
        });

        // GameParticipant with PayoutTxHash
        modelBuilder.Entity<GameParticipant>(entity =>
        {
            entity.ToTable("GameParticipants");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.GameId);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => new { e.GameId, e.UserId }).IsUnique();

            entity.Property(e => e.PayoutAmount).HasPrecision(18, 8);

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
            entity.ToTable("GameMoves");
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
            entity.ToTable("ImageSets");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.Difficulty);
        });

        // ImagePair configuration
        modelBuilder.Entity<ImagePair>(entity =>
        {
            entity.ToTable("ImagePairs");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.SetId);

            entity.HasOne(e => e.Set)
                .WithMany(s => s.ImagePairs)
                .HasForeignKey(e => e.SetId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Coupon configuration (simplified)
        modelBuilder.Entity<Coupon>(entity =>
        {
            entity.ToTable("Coupons");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => new { e.IsActive, e.ExpiresAt });

            entity.Property(e => e.Value).HasPrecision(18, 8);
        });

        // CouponRedemption (renamed from CouponUsage)
        modelBuilder.Entity<CouponRedemption>(entity =>
        {
            entity.ToTable("CouponRedemptions");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CouponId);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.BlockchainTxHash);
            entity.HasIndex(e => e.UserWalletAddress);

            entity.Property(e => e.RedeemedAmount).HasPrecision(18, 8);

            entity.HasOne(e => e.Coupon)
                .WithMany(c => c.Redemptions)
                .HasForeignKey(e => e.CouponId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // AffiliateLink configuration
        modelBuilder.Entity<AffiliateLink>(entity =>
        {
            entity.ToTable("AffiliateLinks");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ReferrerUserId);
            entity.HasIndex(e => e.ReferredUserId).IsUnique();

            entity.Property(e => e.TotalEarnings).HasPrecision(18, 8);

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
            entity.ToTable("AffiliatePayouts");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.LinkId);
            entity.HasIndex(e => e.GameId);
            entity.HasIndex(e => e.BlockchainTxHash);

            entity.Property(e => e.Amount).HasPrecision(18, 8);

            entity.HasOne(e => e.Link)
                .WithMany(l => l.Payouts)
                .HasForeignKey(e => e.LinkId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // AuditLog configuration
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("AuditLogs");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.CreatedAt });
            entity.HasIndex(e => new { e.Level, e.CreatedAt });
            entity.HasIndex(e => new { e.EntityType, e.EntityId });
        });

        // GameSession configuration
        modelBuilder.Entity<GameSession>(entity =>
        {
            entity.ToTable("GameSessions");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.GameId, e.IsActive });
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.ConnectionId);
        });
    }
}