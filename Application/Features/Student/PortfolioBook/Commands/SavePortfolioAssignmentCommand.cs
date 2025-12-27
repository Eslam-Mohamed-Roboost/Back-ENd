using API.Application.Features.Student.PortfolioBook.DTOs;
using API.Domain.Entities.Portfolio;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Student.PortfolioBook.Commands;

public record SavePortfolioAssignmentCommand(SavePortfolioAssignmentRequest Request) : IRequest<RequestResult<PortfolioAssignmentDto>>;

public class SavePortfolioAssignmentCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<PortfolioBookAssignment> assignmentRepository)
    : RequestHandlerBase<SavePortfolioAssignmentCommand, RequestResult<PortfolioAssignmentDto>>(parameters)
{
    public override async Task<RequestResult<PortfolioAssignmentDto>> Handle(SavePortfolioAssignmentCommand request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // Option B: create-only (no updates)
        if (request.Request.Id.HasValue)
        {
            return RequestResult<PortfolioAssignmentDto>.Failure(ErrorCode.BadRequest, "Assignments cannot be edited after saving.");
        }

        var entity = new PortfolioBookAssignment
        {
            StudentId = studentId,
            SubjectId = request.Request.SubjectId,
            Name = request.Request.Name,
            DueDate = request.Request.DueDate,
            Status = request.Request.Status,
            Notes = request.Request.Notes
        };

        assignmentRepository.Add(entity);
        await assignmentRepository.SaveChangesAsync(cancellationToken);

        var assignment = new PortfolioAssignmentDto
        {
            Id = entity.ID,
            Name = request.Request.Name,
            DueDate = request.Request.DueDate,
            Status = request.Request.Status,
            Notes = request.Request.Notes,
            Grade = entity.Grade
        };

        return RequestResult<PortfolioAssignmentDto>.Success(assignment);
    }
}
