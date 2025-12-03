using API.Domain.UnitOfWork;

namespace API.Middlewares;

public class TransactionMiddleware : IMiddleware
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionMiddleware(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Method == HttpMethods.Get)
        {
            await next(context);
            return;
        }

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            await next(context);

            if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
            {
                await _unitOfWork.CommitChangesAsync();
            }
            else
            {
                await _unitOfWork.RollbackTransactionAsync();
            }
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}

