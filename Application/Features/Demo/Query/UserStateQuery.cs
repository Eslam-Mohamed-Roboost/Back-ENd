using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Demo.Query
{
    public record UserStateQuery:IRequest<RequestResult<string>>;
    public class UserStateQueryHandler : RequestHandlerBase<UserStateQuery, RequestResult<string>>
    {
        public UserStateQueryHandler(RequestHandlerBaseParameters parameters) : base(parameters)
        {
        }

        public async  override Task<RequestResult<string>> Handle(UserStateQuery request, CancellationToken cancellationToken)
        {
            string userNmae = _userState.Username;
            var x = RequestResult<string>.Success(userNmae);
            return  x;
        }
    }
}
