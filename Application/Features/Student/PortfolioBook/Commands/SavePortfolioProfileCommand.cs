using API.Application.Features.Student.PortfolioBook.DTOs;
using API.Domain.Entities.Portfolio;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Student.PortfolioBook.Commands;

public record SavePortfolioProfileCommand(SavePortfolioProfileRequest Request) : IRequest<RequestResult<PortfolioProfileDto>>;

public class SavePortfolioProfileCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<PortfolioBookProfile> profileRepository)
    : RequestHandlerBase<SavePortfolioProfileCommand, RequestResult<PortfolioProfileDto>>(parameters)
{
    public override async Task<RequestResult<PortfolioProfileDto>> Handle(SavePortfolioProfileCommand request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        var alreadySubmitted = await profileRepository.Get(x => x.StudentId == studentId && x.SubjectId == request.Request.SubjectId)
            .AnyAsync(cancellationToken);

        if (alreadySubmitted)
        {
            return RequestResult<PortfolioProfileDto>.Failure(ErrorCode.BadRequest, "You have already submitted your profile.");
        }

        var entity = new PortfolioBookProfile
        {
            StudentId = studentId,
            SubjectId = request.Request.SubjectId,
            FullName = request.Request.FullName,
            GradeSection = request.Request.GradeSection,
            FavoriteThings = request.Request.FavoriteThings,
            Uniqueness = request.Request.Uniqueness,
            FutureDream = request.Request.FutureDream
        };

        profileRepository.Add(entity);
        await profileRepository.SaveChangesAsync(cancellationToken);

        var profile = new PortfolioProfileDto
        {
            FullName = request.Request.FullName,
            GradeSection = request.Request.GradeSection,
            FavoriteThings = request.Request.FavoriteThings,
            Uniqueness = request.Request.Uniqueness,
            FutureDream = request.Request.FutureDream
        };

        return RequestResult<PortfolioProfileDto>.Success(profile);
    }
}
