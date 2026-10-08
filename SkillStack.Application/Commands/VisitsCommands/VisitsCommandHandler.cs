using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillStack.Domain.Entities;
using SkillStack.Infrastructure.Persistence;
using SkillStack.Infrastructure.Services;

namespace SkillStack.Application.Commands.VisitsCommands
{
    public class VisitsCommandHandler : IRequestHandler<VisitsCommand, Unit>
    {
        // Rotas aceitas. Ajuste para as do seu portfólio.
        private static readonly HashSet<string> AllowedPaths =
            ["/home", "/problem-solving-showcase", "/technologies", "/article"];

        private readonly AppDbContext _dbContext;
        private readonly TelegramNotifierService _telegram;

        public VisitsCommandHandler(AppDbContext dbContext, TelegramNotifierService telegram)
        {
            _dbContext = dbContext;
            _telegram = telegram;
        }

        public async Task<Unit> Handle(VisitsCommand command, CancellationToken cancellationToken)
        {
            var ua = command.UserAgent.ToLowerInvariant();
            if (ua.Contains("bot") || ua.Contains("crawl") || ua.Contains("spider"))
                return Unit.Value;

            var path = command.Path.Split('?')[0];
            if (command.VisitId == Guid.Empty || !AllowedPaths.Contains(path))
                return Unit.Value;

            var device = ua.Contains("mobi") ? "mobile" : "desktop";

            string? referrer = null;
            if (Uri.TryCreate(command.Referrer, UriKind.Absolute, out var uri))
                referrer = uri.Host.Length > 100 ? uri.Host[..100] : uri.Host; // só o domínio

            var isFirstOfVisit = !await _dbContext.Visits
                .AnyAsync(v => v.VisitId == command.VisitId, cancellationToken);

            _dbContext.Visits.Add(new Visit
            {
                VisitId = command.VisitId,
                Path = path,
                Device = device,
                Referrer = referrer,
                CreatedAtUtc = DateTime.UtcNow
            });
            await _dbContext.SaveChangesAsync(cancellationToken);

            if (isFirstOfVisit)
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
                var hora = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
                await _telegram.SendAsync(
                    $"Nova visita às {hora:HH:mm} em {path} ({device}) — origem: {referrer ?? "direto"}");
            }

            return Unit.Value;
        }
    }
}