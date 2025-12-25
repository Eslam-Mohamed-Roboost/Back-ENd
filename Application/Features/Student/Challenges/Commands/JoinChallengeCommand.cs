using API.Domain.Entities.Gamification;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Student.Challenges.Commands;

public record JoinChallengeCommand(long ChallengeId) : IRequest<RequestResult<bool>>;

public class JoinChallengeCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.Gamification.Challenges> challengesRepository,
    IRepository<StudentChallenges> studentChallengesRepository)
    : RequestHandlerBase<JoinChallengeCommand, RequestResult<bool>>(parameters)
{
    public override async Task<RequestResult<bool>> Handle(JoinChallengeCommand request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // Check if challenge exists and is active
        var challenge = await challengesRepository.Get(x => x.ID == request.ChallengeId && x.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        if (challenge == null)
            return RequestResult<bool>.Failure(ErrorCode.NotFound, "Challenge not found");

        // Check if already joined
        var existing = await studentChallengesRepository.Get(x => 
            x.StudentId == studentId && 
            x.ChallengeId == request.ChallengeId)
            .FirstOrDefaultAsync(cancellationToken);

        if (existing != null)
            return RequestResult<bool>.Failure(ErrorCode.BadRequest, "Already joined this challenge");

        // Join the challenge
        var studentChallenge = new StudentChallenges
        {
            StudentId = studentId,
            ChallengeId = request.ChallengeId,
            Status = ProgressStatus.InProgress
        };

        studentChallengesRepository.Add(studentChallenge);

        return RequestResult<bool>.Success(true);
    }
}
