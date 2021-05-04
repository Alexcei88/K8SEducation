using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using OTUS.HomeWork.DeliveryService.Domain;
using System;
using System.Threading.Tasks;
using OTUS.HomeWork.DeliveryService.DAL;

namespace OTUS.HomeWork.DeliveryService.Services
{
    public class DeliveryService
    {
        public DeliveryContext _context;

        public DeliveryService(DeliveryContext context)
        {
            _context = context;
        }
    
        public Delivery CalculateDelivery(Delivery delivery)
        {
            // здесь какая-то логика по примерному расчету доставки
            delivery.Location.ShipmentDate = DateTime.UtcNow.AddHours(8);
            delivery.Location.EstimatedDate = DateTime.UtcNow.AddDays(3);
            return delivery;
        }

        public async Task<Delivery> CreateDeliveryAsync(Delivery delivery)
        {
            var existDelivery = await _context.Delivery.FirstOrDefaultAsync(g => g.OrderNumber == delivery.OrderNumber);
            if (existDelivery != null)
                return existDelivery;

            foreach(var prod in delivery.Products)
            {
                prod.OrderNumber = delivery.OrderNumber;
            }
            delivery = CalculateDelivery(delivery);
            if (delivery == null)
                return null;

            delivery.Location.CurrentAddress = "На складе";
            try
            {
                _context.Delivery.Add(delivery);
                await _context.SaveChangesAsync();
                return delivery;
            }
            // unique constraints
            catch (UniqueConstraintException)
            {
                return await _context.Delivery.FirstAsync(g => g.OrderNumber == delivery.OrderNumber);
            }
        }

        public async Task<DeliveryLocation> GetLocationOfOrderAsync(string orderNumber)
        {
            return await _context.Location.FirstOrDefaultAsync(g => g.OrderNumber == orderNumber);
        }

        public async Task<DeliveryLocation> UpdateLocationOfOrderAsync(string orderNumber, string newAddress)
        {
            var dbLocation = await _context.Location.FirstOrDefaultAsync(g => g.OrderNumber == orderNumber);
            if(dbLocation == null)
            {
                throw new Exception("Location is not found");
            }
            dbLocation.CurrentAddress = newAddress;
            // другие параметры могут пересчитаться в зависимости от адреса
            await _context.SaveChangesAsync();
            return dbLocation;
        }
    }
}
