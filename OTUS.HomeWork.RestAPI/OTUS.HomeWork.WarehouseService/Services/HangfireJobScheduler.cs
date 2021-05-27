using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OTUS.HomeWork.WarehouseService.HangfireJobs;
using OTUS.HomeWork.WarehouseService.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OTUS.HomeWork.WarehouseService.Services
{
    public class HangfireJobScheduler
        : IHostedService
        , IDisposable
    {
        private const string RESERVE_PRODUCT_JOB_ID = "ReserverProductJobId";
        private ScheduleJobsOption _scheduleOptions;
        private IRecurringJobManager _recurringJobManager;
        private CancellationTokenSource _cancelTokenSource;

        public HangfireJobScheduler(
            IRecurringJobManager recurringJobManager, 
            IOptions<ScheduleJobsOption> scheduleOptions)
        {
            _scheduleOptions = scheduleOptions.Value;
            _recurringJobManager = recurringJobManager;
       }

        public void Dispose()
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancelTokenSource = new CancellationTokenSource();

            _recurringJobManager.AddOrUpdate<ReserveProductTrackerJob>(RESERVE_PRODUCT_JOB_ID,  
                g => g.ResetReserveProductsAsync(new TimeSpan(0, 0, _scheduleOptions.ReserveProductOldTime), _cancelTokenSource.Token), _scheduleOptions.ReserveProductCronExpr);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancelTokenSource.Cancel();
            _recurringJobManager.RemoveIfExists(RESERVE_PRODUCT_JOB_ID);
            return Task.CompletedTask;
        }
    }
}
