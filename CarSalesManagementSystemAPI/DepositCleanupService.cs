using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using DataAccessObjects;
using BusinessObjects.Models;

namespace CarSalesManagementSystemAPI;

public class DepositCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DepositCleanupService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

    public DepositCleanupService(IServiceScopeFactory scopeFactory, ILogger<DepositCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Deposit Cleanup Background Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await DoCleanupAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing deposit cleanup.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("Deposit Cleanup Background Service is stopping.");
    }

    private async Task DoCleanupAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        using var dbContext = new CarShowroomContext();

        var now = DateTime.Now;

        var expiredDeposits = await dbContext.PurchaseRequests
            .Include(p => p.Car)
            .Where(p => p.Status == "Pending" && p.DepositExpiry != null && p.DepositExpiry < now)
            .ToListAsync();

        if (expiredDeposits.Any())
        {
            _logger.LogInformation("Found {Count} expired deposits to clean up.", expiredDeposits.Count);

            foreach (var deposit in expiredDeposits)
            {
                _logger.LogInformation("Expiring deposit RequestId: {RequestId} for CarId: {CarId} (Expiry was: {Expiry}). Refund forfeited.",
                    deposit.RequestId, deposit.CarId, deposit.DepositExpiry);

                deposit.Status = "Rejected";
                deposit.UpdatedAt = now;

                if (deposit.Car != null)
                {
                    deposit.Car.Status = "Available";
                    _logger.LogInformation("Restored CarId: {CarId} status to Available.", deposit.CarId);
                }
            }

            await dbContext.SaveChangesAsync();
            _logger.LogInformation("Successfully updated expired deposits and restored car stock.");
        }
    }
}
