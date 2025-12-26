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
    IRepository<PortfolioBookReflection> reflectionRepository,
    IRepository<PortfolioBookJourneyEntry> journeyRepository)
    : RequestHandlerBase<GetPortfolioBookQuery, RequestResult<PortfolioBookDto>>(parameters)
{
    public override async Task<RequestResult<PortfolioBookDto>> Handle(GetPortfolioBookQuery request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

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
            Reflections = reflections,
            JourneyEntries = journeyEntries,
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
