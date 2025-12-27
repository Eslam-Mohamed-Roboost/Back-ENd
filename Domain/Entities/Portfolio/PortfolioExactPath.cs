namespace API.Domain.Entities.Portfolio;

public class PortfolioExactPath : BaseEntity
{
    public long StudentId { get; set; }
    public long SubjectId { get; set; }
    
    // Reading Progress
    public string ReadingCurrentLevel { get; set; } = string.Empty;
    public int ReadingLessonsCompleted { get; set; }
    public int ReadingTotalLessons { get; set; }
    public int ReadingMinutesThisWeek { get; set; }
    public string ReadingTargetCompletion { get; set; } = string.Empty;
    
    // Vocabulary Progress
    public string VocabularyCurrentLevel { get; set; } = string.Empty;
    public int VocabularyWordsMastered { get; set; }
    public int VocabularyAccuracyRate { get; set; }
    
    // Grammar Progress
    public string GrammarCurrentLevel { get; set; } = string.Empty;
    public int GrammarLessonsCompleted { get; set; }
    public int GrammarTotalLessons { get; set; }
    public List<string> GrammarFocusAreas { get; set; } = new();
}
