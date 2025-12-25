using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Student.Notebook.Commands;

public record DeleteNotebookEntryCommand(long EntryId) : IRequest<RequestResult<bool>>;

public class DeleteNotebookEntryCommandHandler(
    RequestHandlerBaseParameters parameters)
    : RequestHandlerBase<DeleteNotebookEntryCommand, RequestResult<bool>>(parameters)
{
    public override async Task<RequestResult<bool>> Handle(DeleteNotebookEntryCommand request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // TODO: Implement when NotebookEntry entity is created
        return RequestResult<bool>.Success(true);
    }
}
