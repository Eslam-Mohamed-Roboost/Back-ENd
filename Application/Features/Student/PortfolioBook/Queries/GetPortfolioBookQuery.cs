using API.Application.Features.Student.PortfolioBook.DTOs;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Student.PortfolioBook.Queries;

public record GetPortfolioBookQuery(long SubjectId) : IRequest<RequestResult<PortfolioBookDto>>;

public class GetPortfolioBookQueryHandler(
    RequestHandlerBaseParameters parameters)
    : RequestHandlerBase<GetPortfolioBookQuery, RequestResult<PortfolioBookDto>>(parameters)
{
    public override async Task<RequestResult<PortfolioBookDto>> Handle(GetPortfolioBookQuery request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // TODO: Implement database queries when entities are created
        // Placeholder return with sample data
        var portfolioBook = new PortfolioBookDto
        {
            SubjectId = request.SubjectId,
            SubjectName = "ELA Subject",
            StudentName = _userState.Username ?? "Student",
            AcademicYear = "2024-25",
            Profile = new PortfolioProfileDto
            {
                FullName = _userState.Username ?? "Student",
                GradeSection = "Grade 6-A",
                FavoriteThings = "",
                Uniqueness = "",
                FutureDream = ""
            },
            Goals = new PortfolioGoalsDto
            {
                AcademicGoal = "",
                BehavioralGoal = "",
                PersonalGrowthGoal = "",
                AchievementSteps = "",
                TargetDate = null
            },
            LearningStyle = new PortfolioLearningStyleDto
            {
                LearnsBestBy = "",
                BestTimeToStudy = "",
                FocusConditions = "",
                HelpfulTools = "",
                Distractions = ""
            },
            MapScores = new List<PortfolioMapScoreDto>(),
            ExactPathProgress = new ExactPathProgressDto
            {
                Reading = new ReadingProgressDto(),
                Vocabulary = new VocabularyProgressDto(),
                Grammar = new GrammarProgressDto()
            },
            Assignments = new List<PortfolioAssignmentDto>(),
            Reflections = new List<PortfolioReflectionDto>(),
            JourneyEntries = new List<PortfolioJourneyEntryDto>(),
            Milestones = new List<PortfolioMilestoneDto>(),
            Projects = new List<PortfolioProjectDto>(),
            Progress = new PortfolioProgressDto
            {
                CompletionPercentage = 0,
                PagesCompleted = 0,
                TotalPages = 9,
                ReflectionsThisTerm = 0,
                ProjectsUploaded = 0
            }
        };

        return RequestResult<PortfolioBookDto>.Success(portfolioBook);
    }
}
