namespace OTUS.HomeWork.EShop.Domain
{
    public enum OrderStatus
    {
        /// <summary>
        /// Pending(not payment)
        /// </summary>
        Pending = 10,

        /// <summary>
        /// Processing(was payment)
        /// </summary>
        Processing = 20,

        /// <summary>
        /// Complete(was shipment)
        /// </summary>
        Complete = 30,

        /// <summary>
        /// Cancelled
        /// </summary>
        Cancelled = 40,

        /// <summary>
        /// Error
        /// </summary>
        Error = 50
    }
}