using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Infrastructure.Data;
using PurchaseAssistant.Infrastructure.Services;

namespace PurchaseAssistant.Web.Services;

public class BusinessBackupWorker(IServiceScopeFactory scopes, TimeProvider clock, ILogger<BusinessBackupWorker> logger) : BackgroundService
{
    public static DateTimeOffset NextRun(DateTimeOffset now)
    {
        var india = TimeZoneInfo.FindSystemTimeZoneById("Asia/Kolkata");
        var local = TimeZoneInfo.ConvertTime(now, india);
        var next = local.Date.AddHours(2);
        if (next <= local.DateTime) next = next.AddDays(1);
        return new DateTimeOffset(TimeZoneInfo.ConvertTimeToUtc(next, india));
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(NextRun(clock.GetUtcNow()) - clock.GetUtcNow(), clock, stoppingToken);
                List<Guid> businesses;
                using (var scope = scopes.CreateScope()) businesses = await scope.ServiceProvider.GetRequiredService<AppDbContext>().Businesses
                    .AsNoTracking().Where(x => x.IsActive).Select(x => x.Id).ToListAsync(stoppingToken);
                foreach (var id in businesses)
                {
                    try { using var scope = scopes.CreateScope(); await scope.ServiceProvider.GetRequiredService<BusinessBackupService>().RunAsync(id, "scheduled", null, stoppingToken); }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { throw; }
                    catch (Exception) { logger.LogWarning("Scheduled business backup could not be recorded"); }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception) { logger.LogWarning("Business backup scheduler is unavailable"); await Task.Delay(TimeSpan.FromMinutes(1), clock, stoppingToken); }
        }
    }
}
