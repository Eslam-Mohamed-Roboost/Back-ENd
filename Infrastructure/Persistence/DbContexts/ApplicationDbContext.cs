using API.Domain.Entities;
using API.Domain.Entities.Identity;
using API.Domain.Entities.Log;
using Microsoft.EntityFrameworkCore;

namespace API.Infrastructure.Persistence.DbContexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Token> Tokens { get; set; }
    public DbSet<ActionLog> ActionLogs { get; set; }

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

