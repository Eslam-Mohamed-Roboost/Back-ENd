using API.Domain.Entities;
using API.Domain.Entities.Identity;
using API.Domain.Entities.Gamification;
using API.Domain.Entities.General;
using API.Domain.Entities.Missions;
using API.Domain.Entities.Portfolio;
using API.Domain.Entities.System;
using API.Domain.Entities.Teacher;
using Microsoft.EntityFrameworkCore;
using BadgesEntity = API.Domain.Entities.Gamification.Badges;

namespace API.Infrastructure.Persistence.DbContexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Identity
    public DbSet<User> Users { get; set; }
    public DbSet<Token> Tokens { get; set; }
    
    // Gamification
    public DbSet<BadgesEntity> Badges { get; set; }
    public DbSet<Challenges> Challenges { get; set; }
    public DbSet<StudentBadges> StudentBadges { get; set; }
    public DbSet<StudentLevels> StudentLevels { get; set; }
    public DbSet<StudentChallenges> StudentChallenges { get; set; }
    public DbSet<QuizAttempts> QuizAttempts { get; set; }
    
    // General
    public DbSet<Classes> Classes { get; set; }
    public DbSet<Subjects> Subjects { get; set; }
    
    // Missions
    public DbSet<Missions> Missions { get; set; }
    public DbSet<Activities> Activities { get; set; }
    public DbSet<StudentMissionProgress> StudentMissionProgress { get; set; }
    public DbSet<StudentActivityProgress> StudentActivityProgress { get; set; }
    
    // Portfolio
    public DbSet<PortfolioFiles> PortfolioFiles { get; set; }
    public DbSet<PortfolioReflections> PortfolioReflections { get; set; }
    public DbSet<PortfolioLikes> PortfolioLikes { get; set; }
    public DbSet<PortfolioStatus> PortfolioStatus { get; set; }
    public DbSet<TeacherFeedback> TeacherFeedback { get; set; }
    
    // System
    public DbSet<ActivityLogs> ActivityLogs { get; set; }
    public DbSet<Announcements> Announcements { get; set; }
    public DbSet<Notifications> Notifications { get; set; }
    public DbSet<SystemLogs> SystemLogs { get; set; }
    public DbSet<SystemSettings> SystemSettings { get; set; }
    
    // Teacher
    public DbSet<TeacherBadgeSubmissions> TeacherBadgeSubmissions { get; set; }
    public DbSet<TeacherSubjects> TeacherSubjects { get; set; }
    public DbSet<TeacherCpdProgress> TeacherCpdProgress { get; set; }
    public DbSet<WeeklyChallenges> WeeklyChallenges { get; set; }
    public DbSet<CpdModules> CpdModules { get; set; }
 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Set precision for all decimal properties
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(decimal) && property.GetColumnType() == null)
                {
                    property.SetColumnType("numeric(18,2)");
                }

                if (property.ClrType == typeof(string) && property.GetMaxLength() == null)
                {
                    property.SetMaxLength(250);
                }
            }

            SetDefaultValue(modelBuilder, entityType, "CreatedAt", typeof(DateTime), "NOW()");
            SetDefaultValue(modelBuilder, entityType, "IsDeleted", typeof(bool), "false");
        }

        // Configure relationships to prevent delete cascade
        foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
    }

    private void SetDefaultValue(ModelBuilder modelBuilder, Microsoft.EntityFrameworkCore.Metadata.IMutableEntityType entityType, string propertyName, Type propertyType, string defaultValueSql)
    {
        var property = entityType.GetProperties().FirstOrDefault(p => p.Name == propertyName && p.ClrType == propertyType);
        if (property != null)
            modelBuilder.Entity(entityType.ClrType).Property(propertyName).HasDefaultValueSql(defaultValueSql);
    }
}
