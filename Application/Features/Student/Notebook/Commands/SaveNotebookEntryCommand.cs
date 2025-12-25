using API.Application.Features.Student.Notebook.DTOs;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Student.Notebook.Commands;

public record SaveNotebookEntryCommand(SaveNotebookEntryRequest Request) : IRequest<RequestResult<NotebookEntryDto>>;

public class SaveNotebookEntryCommandHandler(
    RequestHandlerBaseParameters parameters)
    : RequestHandlerBase<SaveNotebookEntryCommand, RequestResult<NotebookEntryDto>>(parameters)
{
    public override async Task<RequestResult<NotebookEntryDto>> Handle(SaveNotebookEntryCommand request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // TODO: Implement when NotebookEntry entity is created
        // Placeholder return
        var entry = new NotebookEntryDto
        {
            Id = request.Request.Id ?? 1,
            Title = request.Request.Title,
            Content = request.Request.Content,
            CreatedDate = DateTime.UtcNow,
            LastModifiedDate = DateTime.UtcNow,
            SubjectId = request.Request.SubjectId,
            Tags = request.Request.Tags,
            IsFavorite = request.Request.IsFavorite
        };

        return RequestResult<NotebookEntryDto>.Success(entry);
    }
}
