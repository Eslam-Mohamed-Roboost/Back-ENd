using System.ComponentModel.DataAnnotations.Schema;

namespace API.Domain.Entities.Portfolio;

[Table("PortfolioBookProjects", Schema = "Portfolio")]
public class PortfolioBookProject : BaseEntity
{
    public long StudentId { get; set; }
    public long SubjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SkillsUsed { get; set; } = string.Empty;
    public string WhatLearned { get; set; } = string.Empty;

    // Stored as jsonb array of urls
    public List<string> FileUrls { get; set; } = new();
}
