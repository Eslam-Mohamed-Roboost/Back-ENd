using API.Helpers.Attributes;
using System.Text.Json.Serialization;

namespace API.Application.Features.Student.PortfolioBook.DTOs;

#region Main Response DTO

public class PortfolioBookDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public PortfolioProfileDto? Profile { get; set; }
    public PortfolioGoalsDto? Goals { get; set; }
    public PortfolioLearningStyleDto? LearningStyle { get; set; }
    public List<PortfolioMapScoreDto> MapScores { get; set; } = new();
    public ExactPathProgressDto? ExactPathProgress { get; set; }
    public List<PortfolioAssignmentDto> Assignments { get; set; } = new();
    public List<PortfolioReflectionDto> Reflections { get; set; } = new();
    public List<PortfolioJourneyEntryDto> JourneyEntries { get; set; } = new();
    public List<PortfolioMilestoneDto> Milestones { get; set; } = new();
    public List<PortfolioProjectDto> Projects { get; set; } = new();
    public PortfolioProgressDto? Progress { get; set; }
}

#endregion

#region Profile DTOs

public class PortfolioProfileDto
{
    public string FullName { get; set; } = string.Empty;
    public string GradeSection { get; set; } = string.Empty;
    public string FavoriteThings { get; set; } = string.Empty;
    public string Uniqueness { get; set; } = string.Empty;
    public string FutureDream { get; set; } = string.Empty;
}

public class SavePortfolioProfileRequest
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long SubjectId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string GradeSection { get; set; } = string.Empty;
    public string FavoriteThings { get; set; } = string.Empty;
    public string Uniqueness { get; set; } = string.Empty;
    public string FutureDream { get; set; } = string.Empty;
}

#endregion

#region Goals DTOs

public class PortfolioGoalsDto
{
    public string AcademicGoal { get; set; } = string.Empty;
    public string BehavioralGoal { get; set; } = string.Empty;
    public string PersonalGrowthGoal { get; set; } = string.Empty;
    public string AchievementSteps { get; set; } = string.Empty;
    public DateTime? TargetDate { get; set; }
}

public class SavePortfolioGoalsRequest
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long SubjectId { get; set; }
    public string AcademicGoal { get; set; } = string.Empty;
    public string BehavioralGoal { get; set; } = string.Empty;
    public string PersonalGrowthGoal { get; set; } = string.Empty;
    public string AchievementSteps { get; set; } = string.Empty;
    public DateTime? TargetDate { get; set; }
}

#endregion

#region Learning Style DTOs

public class PortfolioLearningStyleDto
{
    public string LearnsBestBy { get; set; } = string.Empty;
    public string BestTimeToStudy { get; set; } = string.Empty;
    public string FocusConditions { get; set; } = string.Empty;
    public string HelpfulTools { get; set; } = string.Empty;
    public string Distractions { get; set; } = string.Empty;
}

public class SavePortfolioLearningStyleRequest
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long SubjectId { get; set; }
    public string LearnsBestBy { get; set; } = string.Empty;
    public string BestTimeToStudy { get; set; } = string.Empty;
    public string FocusConditions { get; set; } = string.Empty;
    public string HelpfulTools { get; set; } = string.Empty;
    public string Distractions { get; set; } = string.Empty;
}

#endregion

#region MAP Scores DTOs

public class PortfolioMapScoreDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Term { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Score { get; set; }
    public DateTime DateTaken { get; set; }
    public int? Percentile { get; set; }
    public int? Growth { get; set; }
    public int? GoalScore { get; set; }
}

public class UpdateMapScoreRequest
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long StudentId { get; set; }
    [JsonConverter(typeof(LongAsStringConverter))]
    public long SubjectId { get; set; }
    public string Term { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Score { get; set; }
    public DateTime DateTaken { get; set; }
    public int? Percentile { get; set; }
}

#endregion

#region Exact Path Progress DTOs

public class ExactPathProgressDto
{
    public ReadingProgressDto? Reading { get; set; }
    public VocabularyProgressDto? Vocabulary { get; set; }
    public GrammarProgressDto? Grammar { get; set; }
}

public class ReadingProgressDto
{
    public string CurrentLevel { get; set; } = string.Empty;
    public int LessonsCompleted { get; set; }
    public int TotalLessons { get; set; }
    public int MinutesThisWeek { get; set; }
    public string TargetCompletion { get; set; } = string.Empty;
}

public class VocabularyProgressDto
{
    public string CurrentLevel { get; set; } = string.Empty;
    public int WordsMastered { get; set; }
    public int AccuracyRate { get; set; }
}

public class GrammarProgressDto
{
    public string CurrentLevel { get; set; } = string.Empty;
    public int LessonsCompleted { get; set; }
    public int TotalLessons { get; set; }
    public List<string> FocusAreas { get; set; } = new();
}

public class UpdateExactPathRequest
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long StudentId { get; set; }
    [JsonConverter(typeof(LongAsStringConverter))]
    public long SubjectId { get; set; }
    public ReadingProgressDto? Reading { get; set; }
    public VocabularyProgressDto? Vocabulary { get; set; }
    public GrammarProgressDto? Grammar { get; set; }
}

#endregion

#region Assignment DTOs

public class PortfolioAssignmentDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public class SavePortfolioAssignmentRequest
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long SubjectId { get; set; }
    [JsonConverter(typeof(LongAsStringConverter))]
    public long? Id { get; set; } // Null for new, provided for update
    public string Name { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

#endregion

#region Reflection DTOs

public class PortfolioReflectionDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public DateTime WeekOf { get; set; }
    public string WhatLearned { get; set; } = string.Empty;
    public string BiggestAchievement { get; set; } = string.Empty;
    public string ChallengesFaced { get; set; } = string.Empty;
    public string HelpNeeded { get; set; } = string.Empty;
    public string Mood { get; set; } = string.Empty; // Excellent, Good, Okay, Challenging, Difficult
}

public class SavePortfolioReflectionRequest
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long SubjectId { get; set; }
    public DateTime WeekOf { get; set; }
    public string WhatLearned { get; set; } = string.Empty;
    public string BiggestAchievement { get; set; } = string.Empty;
    public string ChallengesFaced { get; set; } = string.Empty;
    public string HelpNeeded { get; set; } = string.Empty;
    public string Mood { get; set; } = string.Empty;
}

#endregion

#region Journey Entry DTOs

public class PortfolioJourneyEntryDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public DateTime Date { get; set; }
    public string SkillsWorking { get; set; } = string.Empty;
    public string EvidenceOfLearning { get; set; } = string.Empty;
    public string HowGrown { get; set; } = string.Empty;
    public string NextSteps { get; set; } = string.Empty;
}

public class SavePortfolioJourneyRequest
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long SubjectId { get; set; }
    public DateTime Date { get; set; }
    public string SkillsWorking { get; set; } = string.Empty;
    public string EvidenceOfLearning { get; set; } = string.Empty;
    public string HowGrown { get; set; } = string.Empty;
    public string NextSteps { get; set; } = string.Empty;
}

#endregion

#region Milestone DTOs

public class PortfolioMilestoneDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
}

#endregion

#region Project DTOs

public class PortfolioProjectDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Writing Project, Research Report, etc.
    public string Description { get; set; } = string.Empty;
    public string SkillsUsed { get; set; } = string.Empty;
    public string WhatLearned { get; set; } = string.Empty;
    public List<string> FileUrls { get; set; } = new();
    public DateTime CreatedDate { get; set; }
}

public class SavePortfolioProjectRequest
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long SubjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SkillsUsed { get; set; } = string.Empty;
    public string WhatLearned { get; set; } = string.Empty;
    // Files handled separately via multipart/form-data
}

#endregion

#region Progress DTOs

public class PortfolioProgressDto
{
    public int CompletionPercentage { get; set; }
    public int PagesCompleted { get; set; }
    public int TotalPages { get; set; }
    public int ReflectionsThisTerm { get; set; }
    public int ProjectsUploaded { get; set; }
}

#endregion
