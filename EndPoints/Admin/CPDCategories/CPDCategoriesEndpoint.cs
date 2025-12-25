using API.Application.Features.Admin.CPD.DTOs;
using API.Application.Features.Admin.CPD.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.CPDCategories;

public class CPDCategoriesEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/CPD/Categories",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetCPDCategoriesQuery(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .Produces<EndPointResponse<List<CPDCategoryDto>>>()
            .AddEndpointFilter<JwtEndpointFilter>();
    }
}
