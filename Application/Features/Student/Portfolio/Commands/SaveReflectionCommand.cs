using API.Application.Features.Student.Portfolio.DTOs;
using API.Domain.Entities.Portfolio;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Student.Portfolio.Commands;

public record SaveReflectionCommand(SaveReflectionRequest Request) : IRequest<RequestResult<ReflectionDto>>;

public class SaveReflectionCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<PortfolioReflections> reflectionsRepository)
    : RequestHandlerBase<SaveReflectionCommand, RequestResult<ReflectionDto>>(parameters)
{
    public override async Task<RequestResult<ReflectionDto>> Handle(SaveReflectionCommand request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        var reflection = new PortfolioReflections
        {
            StudentId = studentId,
            SubjectId = request.Request.SubjectId,
            Content = request.Request.Content,
            Prompt = request.Request.Prompt,
            IsAutoSaved = false
        };

         reflectionsRepository.Add(reflection);

        var result = new ReflectionDto
        {
            Id = reflection.ID,
            Content = reflection.Content,
            Date = reflection.CreatedAt,
            Prompt = reflection.Prompt,
            SubjectId = reflection.SubjectId,
            AutoSaved = reflection.IsAutoSaved
        };

        return RequestResult<ReflectionDto>.Success(result);
    }
}
