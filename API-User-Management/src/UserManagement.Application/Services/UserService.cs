using UserManagement.Application.Abstractions;
using UserManagement.Application.DTOs;
using UserManagement.Application.Exceptions;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Services;

public sealed class UserService(IUserRepository repository) : IUserService
{
    public async Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var users = await repository.GetAllAsync(cancellationToken);
        return users.Select(Map).ToList();
    }

    public async Task<UserResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await repository.GetByIdAsync(id, cancellationToken)
                   ?? throw new NotFoundException($"User '{id}' was not found.");

        return Map(user);
    }

    public async Task<UserResponse> CreateAsync(
        CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var user = new User(request.Name , request.Age, request.City, request.State, request.Pincode);
        await repository.AddAsync(user, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return Map(user);
    }


    public async Task UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await repository.GetByIdAsync(id, cancellationToken)
                   ?? throw new NotFoundException($"User '{request.Name}' was not found.");
        if(user != null)
            user.Update(request.Name, request.Age, request.City, request.State, request.Pincode);
        await repository.UpdateAsync(user, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await repository.GetByIdAsync(id, cancellationToken)
                   ?? throw new NotFoundException($"User '{id}' was not found.");
        if(user != null)
            repository.DeleteAsync(user, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
    }
    private static UserResponse Map(User user) =>
        new(user.Id, user.Name, user.Age, user.City, user.State, user.Pincode, user.CreatedAtUtc);
}
