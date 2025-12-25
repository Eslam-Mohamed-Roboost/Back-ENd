using API.Domain.Entities.Portfolio;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Portfolio.Commands;

public record TogglePortfolioLikeCommand(long StudentId, long SubjectId) : IRequest<RequestResult<bool>>;

public class TogglePortfolioLikeCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<PortfolioLikes> likesRepository)
    : RequestHandlerBase<TogglePortfolioLikeCommand, RequestResult<bool>>(parameters)
{
    public override async Task<RequestResult<bool>> Handle(TogglePortfolioLikeCommand request, CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        var existing = await likesRepository.Get(x =>
                x.StudentId == request.StudentId &&
                x.SubjectId == request.SubjectId &&
                x.TeacherId == teacherId)
            .FirstOrDefaultAsync(cancellationToken);

        if (existing == null)
        {
            var like = new PortfolioLikes
            {
                TeacherId = teacherId,
                StudentId = request.StudentId,
                SubjectId = request.SubjectId
            };
            likesRepository.Add(like);
            await likesRepository.SaveChangesAsync();
            return RequestResult<bool>.Success(true);
        }

        likesRepository.Delete(existing);
        await likesRepository.SaveChangesAsync();
        return RequestResult<bool>.Success(false);
    }
}


