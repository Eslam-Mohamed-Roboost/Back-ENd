using MediatR;

namespace API.Shared.Models;

public class RequestHandlerBaseParameters
{
    public IMediator Mediator => _mediator;
    public UserState UserState => _userState;

    private readonly UserState _userState;
    private readonly IMediator _mediator;

    public RequestHandlerBaseParameters(IMediator mediator, UserState userState)
    {
        _mediator = mediator;
        _userState = userState;
    }
}

