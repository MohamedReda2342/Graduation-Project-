using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApi.Services;

public class FcmNotificationService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly TimeSpan _interval;

    public FcmNotificationService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _interval = TimeSpan.FromMinutes(1);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var patientService = serviceProvider.GetRequiredService<IPatientService>();

                try
                {
                    // Call the function you want to run
                    patientService.SendMedicineNotifications(/* Pass required parameters */);

                    // Wait for the specified interval before running the function again
                    await Task.Delay(_interval, stoppingToken);
                }
                catch (Exception ex)
                {
                    // Log or handle the exception appropriately
                }
            }
        }
    }
}
