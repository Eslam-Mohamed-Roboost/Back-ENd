using API.Domain.Entities.Portfolio;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Student.Portfolio.Commands;

public record DeletePortfolioFileCommand(long FileId) : IRequest<RequestResult<bool>>;

public class DeletePortfolioFileCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<PortfolioFiles> portfolioFilesRepository)
    : RequestHandlerBase<DeletePortfolioFileCommand, RequestResult<bool>>(parameters)
{
    public override async Task<RequestResult<bool>> Handle(DeletePortfolioFileCommand request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        var file = await portfolioFilesRepository.Get(x=>x.ID==request.FileId).FirstOrDefaultAsync(cancellationToken);
        
        if (file == null)
            return RequestResult<bool>.Failure(ErrorCode.UserNotFound,"File not found");

        if (file.StudentId != studentId)
            return RequestResult<bool>.Failure(ErrorCode.UserNotFound,"Unauthorized");

         portfolioFilesRepository.Delete(file);

        return RequestResult<bool>.Success(true);
    }
}
