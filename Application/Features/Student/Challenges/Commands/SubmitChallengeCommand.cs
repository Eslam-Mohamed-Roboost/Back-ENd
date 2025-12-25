using API.Application.Events;
using API.Application.Features.Student.Challenges.DTOs;
using API.Application.Services;
using API.Domain.Entities.Gamification;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using DotNetCore.CAP;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Student.Challenges.Commands;

public record SubmitChallengeCommand(long ChallengeId, SubmitChallengeRequest Request) : IRequest<RequestResult<ChallengeSubmissionResponse>>;

public class SubmitChallengeCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<StudentChallenges> studentChallengesRepository,
    IRepository<Domain.Entities.Gamification.Challenges> challengesRepository,
    IHoursTrackingService hoursTrackingService,
    ICapPublisher? eventPublisher = null)
    : RequestHandlerBase<SubmitChallengeCommand, RequestResult<ChallengeSubmissionResponse>>(parameters)
{
    public override async Task<RequestResult<ChallengeSubmissionResponse>> Handle(SubmitChallengeCommand request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // Get student's challenge progress
        var studentChallenge = await studentChallengesRepository.Get(x => 
            x.StudentId == studentId && 
            x.ChallengeId == request.ChallengeId)
            .FirstOrDefaultAsync(cancellationToken);

        if (studentChallenge == null)
            return RequestResult<ChallengeSubmissionResponse>.Failure(ErrorCode.NotFound, "Challenge not joined");

        // Get challenge details
        var challenge = await challengesRepository.Get(x => x.ID == request.ChallengeId)
            .FirstOrDefaultAsync(cancellationToken);

        if (challenge == null)
            return RequestResult<ChallengeSubmissionResponse>.Failure(ErrorCode.NotFound, "Challenge not found");

        // Mark as completed and award points
        studentChallenge.Status = ProgressStatus.Completed;
        studentChallenge.CompletedAt = DateTime.UtcNow;
        studentChallenge.PointsEarned = challenge.Points;
        
        // Simple scoring: if answer is provided, give 80-100% of points
        var score = !string.IsNullOrEmpty(request.Request.Answer) ? 90 : 50;
        studentChallenge.Score = score;

        studentChallengesRepository.Update(studentChallenge);

        // Record learning hours
        var hoursAwarded = await hoursTrackingService.RecordLearningHoursAsync(
            studentId,
            ActivityLogType.Completion,
            request.ChallengeId,
            challenge.HoursAwarded,
            cancellationToken);

        // Publish challenge completed event (if CAP is configured)
        if (eventPublisher != null)
        {
            await eventPublisher.PublishAsync("challenge.completed", new ChallengeCompletedEvent
            {
                UserId = studentId,
                IsTeacher = false,
                ChallengeId = request.ChallengeId,
                ChallengeTitle = challenge.Title,
                BadgeId = challenge.BadgeId,
                HoursAwarded = challenge.HoursAwarded,
                CompletedAt = DateTime.UtcNow
            }, cancellationToken: cancellationToken);
        }

        var response = new ChallengeSubmissionResponse
        {
            Success = true,
            PointsEarned = studentChallenge.PointsEarned,
            BadgeEarned = challenge.BadgeId.HasValue,
            HoursEarned = hoursAwarded,
            Feedback = score >= 80 ? "Great work! You've completed the challenge." : "Challenge submitted. Keep practicing!"
        };

        return RequestResult<ChallengeSubmissionResponse>.Success(response);
    }
}
