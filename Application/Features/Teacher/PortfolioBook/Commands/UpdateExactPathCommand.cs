using API.Application.Features.Student.PortfolioBook.DTOs;
using API.Domain.Entities.Portfolio;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.PortfolioBook.Commands;

public record UpdateExactPathCommand(UpdateExactPathRequest Request) : IRequest<RequestResult<ExactPathProgressDto>>;

public class UpdateExactPathCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<PortfolioExactPath> exactPathRepository)
    : RequestHandlerBase<UpdateExactPathCommand, RequestResult<ExactPathProgressDto>>(parameters)
{
    public override async Task<RequestResult<ExactPathProgressDto>> Handle(UpdateExactPathCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;

        // Check if exact path already exists for this student/subject
        var existing = await exactPathRepository.Get(e =>
            e.StudentId == req.StudentId &&
            e.SubjectId == req.SubjectId)
            .FirstOrDefaultAsync(cancellationToken);

        PortfolioExactPath entity;
        if (existing != null)
        {
            // Update existing
            if (req.Reading != null)
            {
                existing.ReadingCurrentLevel = req.Reading.CurrentLevel;
                existing.ReadingLessonsCompleted = req.Reading.LessonsCompleted;
                existing.ReadingTotalLessons = req.Reading.TotalLessons;
                existing.ReadingMinutesThisWeek = req.Reading.MinutesThisWeek;
                existing.ReadingTargetCompletion = req.Reading.TargetCompletion;
            }
            if (req.Vocabulary != null)
            {
                existing.VocabularyCurrentLevel = req.Vocabulary.CurrentLevel;
                existing.VocabularyWordsMastered = req.Vocabulary.WordsMastered;
                existing.VocabularyAccuracyRate = req.Vocabulary.AccuracyRate;
            }
            if (req.Grammar != null)
            {
                existing.GrammarCurrentLevel = req.Grammar.CurrentLevel;
                existing.GrammarLessonsCompleted = req.Grammar.LessonsCompleted;
                existing.GrammarTotalLessons = req.Grammar.TotalLessons;
                existing.GrammarFocusAreas = req.Grammar.FocusAreas;
            }
            entity = existing;
        }
        else
        {
            // Create new
            entity = new PortfolioExactPath
            {
                StudentId = req.StudentId,
                SubjectId = req.SubjectId,
                ReadingCurrentLevel = req.Reading?.CurrentLevel ?? "",
                ReadingLessonsCompleted = req.Reading?.LessonsCompleted ?? 0,
                ReadingTotalLessons = req.Reading?.TotalLessons ?? 0,
                ReadingMinutesThisWeek = req.Reading?.MinutesThisWeek ?? 0,
                ReadingTargetCompletion = req.Reading?.TargetCompletion ?? "",
                VocabularyCurrentLevel = req.Vocabulary?.CurrentLevel ?? "",
                VocabularyWordsMastered = req.Vocabulary?.WordsMastered ?? 0,
                VocabularyAccuracyRate = req.Vocabulary?.AccuracyRate ?? 0,
                GrammarCurrentLevel = req.Grammar?.CurrentLevel ?? "",
                GrammarLessonsCompleted = req.Grammar?.LessonsCompleted ?? 0,
                GrammarTotalLessons = req.Grammar?.TotalLessons ?? 0,
                GrammarFocusAreas = req.Grammar?.FocusAreas ?? new List<string>()
            };
            exactPathRepository.Add(entity);
        }

        await exactPathRepository.SaveChangesAsync(cancellationToken);

        var dto = new ExactPathProgressDto
        {
            Reading = new ReadingProgressDto
            {
                CurrentLevel = entity.ReadingCurrentLevel,
                LessonsCompleted = entity.ReadingLessonsCompleted,
                TotalLessons = entity.ReadingTotalLessons,
                MinutesThisWeek = entity.ReadingMinutesThisWeek,
                TargetCompletion = entity.ReadingTargetCompletion
            },
            Vocabulary = new VocabularyProgressDto
            {
                CurrentLevel = entity.VocabularyCurrentLevel,
                WordsMastered = entity.VocabularyWordsMastered,
                AccuracyRate = entity.VocabularyAccuracyRate
            },
            Grammar = new GrammarProgressDto
            {
                CurrentLevel = entity.GrammarCurrentLevel,
                LessonsCompleted = entity.GrammarLessonsCompleted,
                TotalLessons = entity.GrammarTotalLessons,
                FocusAreas = entity.GrammarFocusAreas
            }
        };

        return RequestResult<ExactPathProgressDto>.Success(dto);
    }
}
