using API.Application.Features.User.GetUsers.Queriess;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.GetAdminKPI.Queries;

public class GetAdminKPIRespone
{
    public int TotalUsers { get; set; }
    public int BadgesEarned { get; set; }
    public int ThisWeekActivity {get; set; }

}
public record GetAdminKpiQuery : IRequest<RequestResult<GetAdminKPIRespone>>;
public class GetAdminKpiQueryHandler(RequestHandlerBaseParameters parameters, IRepository< Domain.Entities.User> _repository)
    : RequestHandlerBase<GetAdminKpiQuery, RequestResult<GetAdminKPIRespone>>(parameters)
{

    public override async Task<RequestResult<GetAdminKPIRespone>> Handle(GetAdminKpiQuery request, CancellationToken cancellationToken)
    {
      var result =   await _repository.Get()
            .GroupBy(x => 1)
            .Select(x => new GetAdminKPIRespone
            {
                TotalUsers = x.Count(),
                BadgesEarned = x.Select(x => x.Badges.Count()).Sum(),
                ThisWeekActivity = 500000
            }).FirstOrDefaultAsync(cancellationToken);
      
      return RequestResult<GetAdminKPIRespone>.Success(result);
    }
}