using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OTUS.HomeWork.WarehouseService.DAL;
using OTUS.HomeWork.WarehouseService.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OTUS.HomeWork.WarehouseService.HangfireJobs
{
    /// <summary>
    /// Следит за резервом товаров, которые устарели
    /// </summary>
    public class ReserveProductTrackerJob
    {
        private readonly WarehouseContext _warehouseContext;
        private readonly Services.WarehouseService _warehouseService;

        public ReserveProductTrackerJob(WarehouseContext warehouseContext
            , Services.WarehouseService warehouseService)
        {
            _warehouseContext = warehouseContext;
            _warehouseService = warehouseService;
        }

        public async Task ResetReserveProductsAsync(TimeSpan oldReserveTime, CancellationToken token)
        {
            const int page = 30;
            var date = DateTime.UtcNow - oldReserveTime;
            int count = 0;
            do
            {
                var orderNumbers = await _warehouseContext.Reserves
                    .Where(g => g.ReserveDate < date).Select(g=> g.OrderNumber)
                    .OrderBy(g => g)
                    .Skip(count).Take(page)
                    .Distinct().ToArrayAsync();
                if (orderNumbers.Length <= 0)
                    break;

                foreach(var g in orderNumbers)
                {
                    await _warehouseService.ResetReserveProductsAsync(g);
                }

                count += orderNumbers.Length;
                if (token.IsCancellationRequested)
                    break;                
            }
            while (true);
        }
    }
}
