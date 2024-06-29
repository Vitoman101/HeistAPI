using HeistAPI.Data;
using HeistAPI.Entities.Enum;

namespace HeistAPI.Services.BackgroundWorker
{
    public class HeistStatusService : BackgroundService
    {
        private readonly ILogger<HeistStatusService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        
        public HeistStatusService(ILogger<HeistStatusService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Heist Status Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                
                UpdateHeistStatuses();
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }

            _logger.LogInformation("Heist Status Service is stopping.");
        }

        private void UpdateHeistStatuses()
        {
            
            using (var scope = _scopeFactory.CreateScope())
            {
                
                var context = scope.ServiceProvider.GetRequiredService<DataContext>();

                var now = DateTime.UtcNow;

                
                var heistsToStart = context.Heists
                    .Where(h => h.StartTime <= now && h.Status == Status.PLANNING)
                    .ToList();

                foreach (var heist in heistsToStart)
                {
                    heist.Status = Status.IN_PROGRESS;
                }

                var heistsToFinish = context.Heists
                    .Where(h => h.EndTime <= now && h.Status == Status.IN_PROGRESS)
                    .ToList();

                foreach (var heist in heistsToFinish)
                {
                    heist.Status = Status.FINISHED;
                }

                
                context.SaveChanges();
            }
        }
    }

}
