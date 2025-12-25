using API.Application.Features.Admin.Settings.DTOs;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.Settings.Queries;

public record GetSystemSettingsQuery : IRequest<RequestResult<SystemSettingsDto>>;

public class GetSystemSettingsQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.Gamification.Badges> badgesRepository)
    : RequestHandlerBase<GetSystemSettingsQuery, RequestResult<SystemSettingsDto>>(parameters)
{
    public override async Task<RequestResult<SystemSettingsDto>> Handle(GetSystemSettingsQuery request, CancellationToken cancellationToken)
    {
        // Get badge categories from actual badges
        var badgeCategories = await badgesRepository.Get()
            .Where(b => b.IsActive)
            .GroupBy(b => b.Category.ToString())
            .Select(g => new BadgeCategorySettingDto
            {
                Id = $"cat-{g.Key.ToLower()}",
                Name = g.Key,
                BadgeCount = g.Count()
            })
            .ToListAsync(cancellationToken);

        // Settings would typically come from a Settings table, using defaults for now
        var settings = new SystemSettingsDto
        {
            SchoolName = "Abu Dhabi School",
            SchoolLogo = "https://storage.school.ae/logo.png",
            AcademicYear = $"{DateTime.UtcNow.Year}-{DateTime.UtcNow.Year + 1}",
            AdminEmail = "admin@school.ae",
            SupportEmail = "support@school.ae",
            ItSupport = "it@school.ae",
            Timezone = "Asia/Dubai",
            BadgeCategories = badgeCategories,
            CpdTiers = new List<CPDTierDto>
            {
                new() { Tier = 1, Name = "Bronze", BadgeRange = "1-5" },
                new() { Tier = 2, Name = "Silver", BadgeRange = "6-10" },
                new() { Tier = 3, Name = "Gold", BadgeRange = "11+" }
            },
            Missions = new List<MissionSettingDto>
            {
                new() { Id = "m-1", Name = "Digital Citizenship Foundations", Order = 1, Enabled = true },
                new() { Id = "m-2", Name = "AI Tools Mastery", Order = 2, Enabled = true }
            },
            NotificationFrequency = new NotificationFrequencyDto
            {
                BadgeSubmissions = "Immediate",
                ActivitySummary = "Daily"
            },
            BackupSettings = new BackupSettingsDto
            {
                Enabled = true,
                Frequency = "Daily",
                Time = "02:00",
                LastBackup = DateTime.UtcNow.Date.AddHours(2),
                StorageLocation = "Azure Blob Storage"
            }
        };

        return RequestResult<SystemSettingsDto>.Success(settings);
    }
}
