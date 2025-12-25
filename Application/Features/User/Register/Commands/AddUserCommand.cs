using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ClassEntity = API.Domain.Entities.General.Classes;

namespace API.Application.Features.User.Register.Commands;

public record AddUserCommand(
    string Name,
    string UserName,
    string Email,
    string Password,
    string SaltPassword,
    string PhoneNumber,
    ApplicationRole Role,
    long? ClassID = null) : IRequest<long>;

public class AddUserCommandHandler : RequestHandlerBase<AddUserCommand, long>
{
    private readonly IRepository<Domain.Entities.User> _repository;
    private readonly IRepository<ClassEntity> _classRepository;
    
    public AddUserCommandHandler(
        RequestHandlerBaseParameters parameters, 
        IRepository<Domain.Entities.User> repository,
        IRepository<ClassEntity> classRepository) : base(parameters)
    {
        _repository = repository;
        _classRepository = classRepository;
    }

    public override async Task<long> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        // Validate ClassID if provided
        if (request.ClassID.HasValue)
        {
            // Only students should have ClassID
            if (request.Role != ApplicationRole.Student)
            {
                throw new InvalidOperationException("Only students can be assigned to a class");
            }

            var classExists = await _classRepository.Get(c => c.ID == request.ClassID.Value)
                .AnyAsync(cancellationToken);

            if (!classExists)
            {
                throw new InvalidOperationException($"Class with ID {request.ClassID.Value} does not exist");
            }
        }

        long entity = _repository.Add(new Domain.Entities.User
        {
            Name = request.Name,
            UserName = request.UserName,
            Email = request.Email,
            Password = request.Password,
            PhoneNumber = request.PhoneNumber,
            Role = request.Role,
            SaltPassword = request.SaltPassword,
            ClassID = request.ClassID,
            IsActive = true
        });

        await _repository.SaveChangesAsync();

        // Update student count if class was assigned
        if (request.ClassID.HasValue)
        {
            var classEntity = await _classRepository.Get(c => c.ID == request.ClassID.Value)
                .FirstOrDefaultAsync(cancellationToken);

            if (classEntity != null)
            {
                classEntity.StudentCount++;
                _classRepository.Update(classEntity);
                await _classRepository.SaveChangesAsync();
            }
        }

        return await Task.FromResult(entity);
    }
}

