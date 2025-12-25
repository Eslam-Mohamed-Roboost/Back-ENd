using API.Application.Features.Student.Challenges.DTOs;
using API.Domain.Entities.Gamification;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Student.Challenges.Commands;

public record SubmitChallengeCommand(long ChallengeId, SubmitChallengeRequest Request) : IRequest<RequestResult<ChallengeSubmissionResponse>>;

public class SubmitChallengeCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<StudentChallenges> studentChallengesRepository,
    IRepository<Domain.Entities.Gamification.Challenges> challengesRepository)
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

        // TODO: Store answer/attachments if needed
        // TODO: Award badge if applicable

        var response = new ChallengeSubmissionResponse
        {
            Success = true,
            PointsEarned = studentChallenge.PointsEarned,
            BadgeEarned = false, // TODO: Implement badge logic
            Feedback = score >= 80 ? "Great work! You've completed the challenge." : "Challenge submitted. Keep practicing!"
        };

        return RequestResult<ChallengeSubmissionResponse>.Success(response);
    }
}
