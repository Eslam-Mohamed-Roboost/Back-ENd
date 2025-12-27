using API.Application.Features.Student.PortfolioBook.DTOs;
using API.Domain.Entities.Portfolio;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Student.PortfolioBook.Queries;

public record GetPortfolioBookQuery(long SubjectId) : IRequest<RequestResult<PortfolioBookDto>>;

public class GetPortfolioBookQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<PortfolioBookProfile> profileRepository,
    IRepository<PortfolioBookGoals> goalsRepository,
    IRepository<PortfolioBookLearningStyle> learningStyleRepository,
    IRepository<PortfolioBookAssignment> assignmentRepository,
    IRepository<PortfolioBookReflection> reflectionRepository,
    IRepository<PortfolioBookJourneyEntry> journeyRepository,
    IRepository<PortfolioBookProject> projectRepository,
    IRepository<PortfolioMapScore> mapScoreRepository,
    IRepository<PortfolioExactPath> exactPathRepository)
    : RequestHandlerBase<GetPortfolioBookQuery, RequestResult<PortfolioBookDto>>(parameters)
{
    public override async Task<RequestResult<PortfolioBookDto>> Handle(GetPortfolioBookQuery request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        var profileEntity = await profileRepository.Get(x => x.StudentId == studentId && x.SubjectId == request.SubjectId)
            .FirstOrDefaultAsync(cancellationToken);

        var goalsEntity = await goalsRepository.Get(x => x.StudentId == studentId && x.SubjectId == request.SubjectId)
            .FirstOrDefaultAsync(cancellationToken);

        var learningStyleEntity = await learningStyleRepository.Get(x => x.StudentId == studentId && x.SubjectId == request.SubjectId)
            .FirstOrDefaultAsync(cancellationToken);

        var assignments = await assignmentRepository.Get(a => a.StudentId == studentId && a.SubjectId == request.SubjectId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new PortfolioAssignmentDto
            {
                Id = a.ID,
                Name = a.Name,
                DueDate = a.DueDate,
                Status = a.Status,
                Notes = a.Notes,
                Grade = a.Grade
            })
            .ToListAsync(cancellationToken);

        // Fetch reflections and journey entries
        var reflections = await reflectionRepository.Get(r => r.StudentId == studentId && r.SubjectId == request.SubjectId)
            .OrderByDescending(r => r.Date)
            .Select(r => new PortfolioReflectionDto
            {
                Id = r.ID,
                WeekOf = r.WeekOf,
                WhatLearned = r.WhatLearned,
                BiggestAchievement = r.BiggestAchievement,
                ChallengesFaced = r.ChallengesFaced,
                HelpNeeded = r.HelpNeeded,
                Mood = r.Mood
            })
            .ToListAsync(cancellationToken);

        var journeyEntries = await journeyRepository.Get(j => j.StudentId == studentId && j.SubjectId == request.SubjectId)
            .OrderByDescending(j => j.Date)
            .Select(j => new PortfolioJourneyEntryDto
            {
                Id = j.ID,
                Date = j.Date,
                SkillsWorking = j.SkillsWorking,
                EvidenceOfLearning = j.EvidenceOfLearning,
                HowGrown = j.HowGrown,
                NextSteps = j.NextSteps
            })
            .ToListAsync(cancellationToken);

        var projects = await projectRepository.Get(p => p.StudentId == studentId && p.SubjectId == request.SubjectId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PortfolioProjectDto
            {
                Id = p.ID,
                Title = p.Title,
                Type = p.Type,
                Description = p.Description,
                SkillsUsed = p.SkillsUsed,
                WhatLearned = p.WhatLearned,
                FileUrls = p.FileUrls,
                CreatedDate = p.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var mapScores = await mapScoreRepository.Get(m => m.StudentId == studentId && m.SubjectId == request.SubjectId)
            .OrderByDescending(m => m.Year)
            .ThenByDescending(m => m.Term)
            .Select(m => new PortfolioMapScoreDto
            {
                Id = m.ID,
                Term = m.Term,
                Year = m.Year,
                Score = m.Score,
                DateTaken = m.DateTaken,
                Percentile = m.Percentile,
                Growth = m.Growth,
                GoalScore = m.GoalScore
            })
            .ToListAsync(cancellationToken);

        var exactPathEntity = await exactPathRepository.Get(e => e.StudentId == studentId && e.SubjectId == request.SubjectId)
            .FirstOrDefaultAsync(cancellationToken);

        // Placeholder return with sample data
        var portfolioBook = new PortfolioBookDto
        {
            SubjectId = request.SubjectId,
            SubjectName = "ELA Subject",
            StudentName = _userState.Username ?? "Student",
            AcademicYear = "2024-25",
            IsProfileSubmitted = profileEntity != null,
            IsGoalsSubmitted = goalsEntity != null,
            IsLearningStyleSubmitted = learningStyleEntity != null,
            Profile = new PortfolioProfileDto
            {
                FullName = profileEntity?.FullName ?? (_userState.Username ?? "Student"),
                GradeSection = profileEntity?.GradeSection ?? "",
                FavoriteThings = profileEntity?.FavoriteThings ?? "",
                Uniqueness = profileEntity?.Uniqueness ?? "",
                FutureDream = profileEntity?.FutureDream ?? ""
            },
            Goals = new PortfolioGoalsDto
            {
                AcademicGoal = goalsEntity?.AcademicGoal ?? "",
                BehavioralGoal = goalsEntity?.BehavioralGoal ?? "",
                PersonalGrowthGoal = goalsEntity?.PersonalGrowthGoal ?? "",
                AchievementSteps = goalsEntity?.AchievementSteps ?? "",
                TargetDate = goalsEntity?.TargetDate
            },
            LearningStyle = new PortfolioLearningStyleDto
            {
                LearnsBestBy = learningStyleEntity?.LearnsBestBy ?? "",
                BestTimeToStudy = learningStyleEntity?.BestTimeToStudy ?? "",
                FocusConditions = learningStyleEntity?.FocusConditions ?? "",
                HelpfulTools = learningStyleEntity?.HelpfulTools ?? "",
                Distractions = learningStyleEntity?.Distractions ?? ""
            },
            MapScores = mapScores,
            ExactPathProgress = new ExactPathProgressDto
            {
                Reading = new ReadingProgressDto
                {
                    CurrentLevel = exactPathEntity?.ReadingCurrentLevel ?? "",
                    LessonsCompleted = exactPathEntity?.ReadingLessonsCompleted ?? 0,
                    TotalLessons = exactPathEntity?.ReadingTotalLessons ?? 0,
                    MinutesThisWeek = exactPathEntity?.ReadingMinutesThisWeek ?? 0,
                    TargetCompletion = exactPathEntity?.ReadingTargetCompletion ?? ""
                },
                Vocabulary = new VocabularyProgressDto
                {
                    CurrentLevel = exactPathEntity?.VocabularyCurrentLevel ?? "",
                    WordsMastered = exactPathEntity?.VocabularyWordsMastered ?? 0,
                    AccuracyRate = exactPathEntity?.VocabularyAccuracyRate ?? 0
                },
                Grammar = new GrammarProgressDto
                {
                    CurrentLevel = exactPathEntity?.GrammarCurrentLevel ?? "",
                    LessonsCompleted = exactPathEntity?.GrammarLessonsCompleted ?? 0,
                    TotalLessons = exactPathEntity?.GrammarTotalLessons ?? 0,
                    FocusAreas = exactPathEntity?.GrammarFocusAreas ?? new List<string>()
                }
            },
            Assignments = assignments,
            Reflections = reflections,
            JourneyEntries = journeyEntries,
            Milestones = new List<PortfolioMilestoneDto>(),
            Projects = projects,
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
