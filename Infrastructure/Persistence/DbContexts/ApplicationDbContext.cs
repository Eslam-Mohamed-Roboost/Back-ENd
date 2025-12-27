using API.Domain.Entities;
using API.Domain.Entities.Identity;
using API.Domain.Entities.Gamification;
using API.Domain.Entities.General;
using API.Domain.Entities.Missions;
using API.Domain.Entities.Portfolio;
using API.Domain.Entities.System;
using API.Domain.Entities.Teacher;
using API.Domain.Entities.Academic;
using API.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using BadgesEntity = API.Domain.Entities.Gamification.Badges;
using UserBadgesEntity = API.Domain.Entities.Users.Badges;

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
    public DbSet<MissionResources> MissionResources { get; set; }
    
    // Portfolio
    public DbSet<PortfolioFiles> PortfolioFiles { get; set; }
    public DbSet<PortfolioReflections> PortfolioReflections { get; set; }
    public DbSet<PortfolioLikes> PortfolioLikes { get; set; }
    public DbSet<PortfolioStatus> PortfolioStatus { get; set; }
    public DbSet<PortfolioBookProfile> PortfolioBookProfiles { get; set; }
    public DbSet<PortfolioBookGoals> PortfolioBookGoals { get; set; }
    public DbSet<PortfolioBookLearningStyle> PortfolioBookLearningStyles { get; set; }
    public DbSet<PortfolioBookAssignment> PortfolioBookAssignments { get; set; }
    public DbSet<PortfolioBookReflection> PortfolioBookReflections { get; set; }
    public DbSet<PortfolioBookJourneyEntry> PortfolioBookJourneyEntries { get; set; }
    public DbSet<PortfolioBookProject> PortfolioBookProjects { get; set; }
    public DbSet<PortfolioMapScore> PortfolioMapScores { get; set; }
    public DbSet<PortfolioExactPath> PortfolioExactPaths { get; set; }
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
    public DbSet<TeacherClassAssignments> TeacherClassAssignments { get; set; }
    public DbSet<TeacherCpdProgress> TeacherCpdProgress { get; set; }
    public DbSet<WeeklyChallenges> WeeklyChallenges { get; set; }
    public DbSet<CpdModules> CpdModules { get; set; }
    public DbSet<TeacherPermissions> TeacherPermissions { get; set; }
    public DbSet<TeacherMissions> TeacherMissions { get; set; }
    public DbSet<TeacherMissionProgress> TeacherMissionProgress { get; set; }
    public DbSet<TeacherActivityProgress> TeacherActivityProgress { get; set; }
    public DbSet<TeacherActivities> TeacherActivities { get; set; }
    
    // Academic
    public DbSet<Exercises> Exercises { get; set; }
    public DbSet<Examinations> Examinations { get; set; }
    public DbSet<Grades> Grades { get; set; }
    public DbSet<ExerciseSubmissions> ExerciseSubmissions { get; set; }
    public DbSet<ExaminationAttempts> ExaminationAttempts { get; set; }
    public DbSet<StudentAttendance> StudentAttendance { get; set; }
    
    // Users
    public DbSet<UserBadgesEntity> UserBadges { get; set; }
 
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

                if (property.ClrType == typeof(string) && property.GetMaxLength() == null && property.Name != "ResourceLinks")
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

        // Configure JSONB columns
        // WeeklyChallenges ResourceLinks
        modelBuilder.Entity<WeeklyChallenges>()
            .Property(e => e.ResourceLinks)
            .HasColumnType("jsonb");

        modelBuilder.Entity<PortfolioBookProject>()
            .Property(e => e.FileUrls)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
            );

        modelBuilder.Entity<PortfolioExactPath>()
            .Property(e => e.GrammarFocusAreas)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
            );

        // Academic - Exercises Attachments
        modelBuilder.Entity<Exercises>()
            .Property(e => e.Attachments)
            .HasColumnType("jsonb");

        // Academic - Examinations Questions
        modelBuilder.Entity<Examinations>()
            .Property(e => e.Questions)
            .HasColumnType("jsonb");

        // Academic - ExerciseSubmissions Attachments
        modelBuilder.Entity<ExerciseSubmissions>()
            .Property(e => e.Attachments)
            .HasColumnType("jsonb");

        // Academic - ExaminationAttempts Answers
        modelBuilder.Entity<ExaminationAttempts>()
            .Property(e => e.Answers)
            .HasColumnType("jsonb");

        // Teacher - TeacherCpdProgress EvidenceFiles
        modelBuilder.Entity<TeacherCpdProgress>()
            .Property(e => e.EvidenceFiles)
            .HasColumnType("jsonb");

        // System - Announcements TargetAudience
        modelBuilder.Entity<Announcements>()
            .Property(e => e.TargetAudience)
            .HasColumnType("jsonb");

        // Gamification - QuizAttempts Answers
        modelBuilder.Entity<QuizAttempts>()
            .Property(e => e.Answers)
            .HasColumnType("jsonb");
    }

    private void SetDefaultValue(ModelBuilder modelBuilder, Microsoft.EntityFrameworkCore.Metadata.IMutableEntityType entityType, string propertyName, Type propertyType, string defaultValueSql)
    {
        var property = entityType.GetProperties().FirstOrDefault(p => p.Name == propertyName && p.ClrType == propertyType);
        if (property != null)
            modelBuilder.Entity(entityType.ClrType).Property(propertyName).HasDefaultValueSql(defaultValueSql);
    }
}
