namespace OTUS.HomeWork.WarehouseService.Options
{
    public class ScheduleJobsOption
    {
        /// <summary>
        /// Время устаревания резервирования товара
        /// </summary>
        public int ReserveProductOldTime { get; set; }

        public string ReserveProductCronExpr { get; set; }
    }
}
