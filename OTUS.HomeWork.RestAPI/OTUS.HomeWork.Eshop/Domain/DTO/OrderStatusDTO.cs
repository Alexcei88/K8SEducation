namespace OTUS.HomeWork.EShop.Domain.DTO
{
    public enum OrderStatusDTO
    {
        /// <summary>
        /// Pending
        /// </summary>
        Pending = 10,

        /// <summary>
        /// Processing
        /// </summary>
        Processing = 20,

        /// <summary>
        /// Complete
        /// </summary>
        Complete = 30,

        /// <summary>
        /// Cancelled
        /// </summary>
        Cancelled = 40,

        /// <summary>
        /// Error
        /// </summary>
        Error = 50,
    }
}