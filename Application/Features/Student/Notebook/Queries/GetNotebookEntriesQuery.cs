using API.Application.Features.Student.Notebook.DTOs;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Student.Notebook.Queries;

public record GetNotebookEntriesQuery(long? SubjectId, DateTime? DateFrom, DateTime? DateTo) : IRequest<RequestResult<List<NotebookEntryDto>>>;

public class GetNotebookEntriesQueryHandler(
    RequestHandlerBaseParameters parameters)
    : RequestHandlerBase<GetNotebookEntriesQuery, RequestResult<List<NotebookEntryDto>>>(parameters)
{
    public override async Task<RequestResult<List<NotebookEntryDto>>> Handle(GetNotebookEntriesQuery request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // TODO: Implement when NotebookEntry entity is created
        // Placeholder return
        var entries = new List<NotebookEntryDto>();

        return RequestResult<List<NotebookEntryDto>>.Success(entries);
    }
}
