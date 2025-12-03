using API.Domain.Entities.Log;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Common.Logging.Commands;

public record AddActionLogCommand(
    ActionLogType LogType, 
    string Data, 
    string? Note = null) : IRequest<long>;

public class AddActionLogCommandHandler : RequestHandlerBase<AddActionLogCommand, long>
{
    private readonly IRepository<ActionLog> _repository;

    public AddActionLogCommandHandler(RequestHandlerBaseParameters parameters, IRepository<ActionLog> repository) 
        : base(parameters)
    {
        _repository = repository;
    }

    public override async Task<long> Handle(AddActionLogCommand request, CancellationToken cancellationToken)
    {
        var logId = _repository.Add(new ActionLog
        {
            LogType = request.LogType,
            Data = request.Data,
            Note = request.Note ?? string.Empty,
            UserName = _userState.Username
        });

        // Note: Do NOT call SaveChanges here - UnitOfWork handles it
        return await Task.FromResult(logId);
    }
}

