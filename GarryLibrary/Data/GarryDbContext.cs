using DSharpPlus.Entities;
using GarryLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace GarryLibrary.Data;

public class GarryDbContext(DbContextOptions<GarryDbContext> options) : DbContext(options)
{
    public DbSet<ServerConfig> ServerConfigs { get; set; }
    public DbSet<ServerMember> ServerMembers { get; set; }
    public DbSet<WheelPicker> WheelPickers { get; set; }
    public DbSet<WheelOption> WheelOptions { get; set; }
    public DbSet<SantaParticipant> SantaParticipants { get; set; }
    public DbSet<SantaConfig> SantaConfigs { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Ignore<DiscordRole>();
        modelBuilder.Ignore<DiscordMember>();
        modelBuilder.Ignore<DiscordGuild>();

        // ServerConfig - Primary Key
        modelBuilder.Entity<ServerConfig>()
            .HasKey(sc => sc.GuildId);

        // ServerMember - Primary Key and Composite Index
        modelBuilder.Entity<ServerMember>()
            .HasKey(sm => sm.Id);

        modelBuilder.Entity<ServerMember>()
            .HasIndex(sm => new { sm.GuildId, sm.UserId })
            .IsUnique();

        // WheelPicker - One-to-Many with WheelOptions
        modelBuilder.Entity<WheelPicker>()
            .HasKey(wp => wp.Id);

        modelBuilder.Entity<WheelPicker>()
            .HasMany(wp => wp.WheelOptions)
            .WithOne(wo => wo.WheelPicker)
            .HasForeignKey(wo => wo.WheelId)
            .OnDelete(DeleteBehavior.Cascade);

        // WheelOption
        modelBuilder.Entity<WheelOption>()
            .HasKey(wo => new { wo.WheelId, wo.Option });

        // SantaConfig - Primary Key
        modelBuilder.Entity<SantaConfig>()
            .HasKey(sc => sc.GuildId);

        // SantaParticipant - Complex relationships
        modelBuilder.Entity<SantaParticipant>()
            .HasKey(sp => sp.ServerMemberId);

        // SantaParticipant -> ServerMember (main relationship)
        modelBuilder.Entity<SantaParticipant>()
            .HasOne(sp => sp.ServerMember)
            .WithOne(sm => sm.SantaParticipant)
            .HasForeignKey<SantaParticipant>(sp => sp.ServerMemberId)
            .OnDelete(DeleteBehavior.Cascade);

        // SantaParticipant -> Giftee (self-referencing)
        modelBuilder.Entity<SantaParticipant>()
            .HasOne(sp => sp.Giftee)
            .WithMany()
            .HasForeignKey(sp => sp.GifteeId)
            .OnDelete(DeleteBehavior.Restrict);

        // SantaParticipant -> SignificantOther (self-referencing)
        modelBuilder.Entity<SantaParticipant>()
            .HasOne(sp => sp.SignificantOther)
            .WithMany()
            .HasForeignKey(sp => sp.SignificantOtherId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}