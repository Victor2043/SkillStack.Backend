using BCrypt.Net;
using MediatR;
using SkillStack.Application.Commands.UserCommands;
using SkillStack.Domain.Entities;
using SkillStack.Infrastructure.Persistence;

namespace SkillStack.Application.UserCommands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
    {
        private readonly AppDbContext _dbContext;

        public CreateUserCommandHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // Encriptar a senha antes de salvar
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.PasswordHash);

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = request.UserName,
                Name = request.Name,
                Email = request.Email,
                PasswordHash = passwordHash,
                IsActive = false
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return user;
        }
    }
}
