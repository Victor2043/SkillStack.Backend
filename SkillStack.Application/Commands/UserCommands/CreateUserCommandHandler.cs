using System.Net;
using System.Security.Principal;
using BCrypt.Net;
using MediatR;
using Microsoft.Extensions.Configuration;
using SkillStack.Application.Commands.UserCommands;
using SkillStack.Core.Interfaces;
using SkillStack.Domain.Entities;
using SkillStack.Infrastructure.Persistence;

namespace SkillStack.Application.UserCommands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
    {
        private readonly AppDbContext _dbContext;
        private readonly IEmailService _emailService;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;

        public CreateUserCommandHandler(AppDbContext dbContext, IEmailService emailService, IJwtService jwtService, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _emailService = emailService;
            _jwtService = jwtService;
            _configuration = configuration;
        }

        public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
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

            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync(cancellationToken);

                var activationToken = await _jwtService.GenerateActivationToken(user.Id);

                var activationLink = GenerateActivationLink(user.Id, activationToken);

                var emailBody = GenerateEmailBody(activationLink);

               await _emailService.SendEmailAsync(user.Email, "Account Activation - SkillStack", emailBody);

                await transaction.CommitAsync(cancellationToken);
                return user;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        private string GenerateActivationLink(Guid userId, string activationToken)
        {
            var frontendBaseUrl = _configuration["Urls:FrontendBaseUrl"];

            return $"{frontendBaseUrl}/activate?token={WebUtility.UrlEncode(activationToken)}";
        }

        private string GenerateEmailBody(string activationLink)
        {
            return $@"
                <h1>Welcome to SkillStack!</h1>
                <p>Please click the link below to activate your account:</p>
                <a href='{activationLink}'>{activationLink}</a>
                <p>If you didn't request this registration, please ignore this email.</p>
            ";
        }
    }
}
