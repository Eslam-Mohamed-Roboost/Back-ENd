using System.ComponentModel.DataAnnotations.Schema;

namespace API.Domain.Entities.Portfolio;

[Table("PortfolioBookReflections", Schema = "Portfolio")]
public class PortfolioBookReflection : BaseEntity
{
    public long StudentId { get; set; }
    public long SubjectId { get; set; }
    public DateTime Date { get; set; }
    public DateTime WeekOf { get; set; }
    public string WhatLearned { get; set; } = string.Empty;
    public string BiggestAchievement { get; set; } = string.Empty;
    public string ChallengesFaced { get; set; } = string.Empty;
    public string HelpNeeded { get; set; } = string.Empty;
    public string Mood { get; set; } = string.Empty;
}
