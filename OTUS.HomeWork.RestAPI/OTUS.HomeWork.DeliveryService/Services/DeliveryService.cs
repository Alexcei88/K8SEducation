using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using OTUS.HomeWork.DeliveryService.Domain;
using System;
using System.Threading.Tasks;
using OTUS.HomeWork.DeliveryService.DAL;
using OTUS.HomeWork.DeliveryService.Contract.Messages;
using System.Linq;

namespace OTUS.HomeWork.DeliveryService.Services
{
    public class DeliveryService
    {
        public DeliveryContext _context;

        public DeliveryService(DeliveryContext context)
        {
            _context = context;
        }
    
        public Delivery CalculateDelivery(Delivery delivery, DateTime readyToShipment)
        {
            // здесь какая-то логика по примерному расчету доставки
            delivery.Location.ShipmentDate = readyToShipment.AddHours(8);
            delivery.Location.EstimatedDate = DateTime.UtcNow.AddDays(3);
            return delivery;
        }

        public async Task<Delivery> CreateDeliveryAsync(DeliveryOrderRequest request)
        {
            var existDelivery = await _context.Delivery.FirstOrDefaultAsync(g => g.OrderNumber == request.OrderNumber);
            if (existDelivery != null)
                return existDelivery;

            var delivery = new Delivery
            {
                Location = new DeliveryLocation
                {
                    CurrentAddress = "Склад",
                    DeliveryAddress = request.DeliveryAddress,
                },
                OrderNumber = request.OrderNumber,
                Products = request.Products.Select(g => new DeliveryProduct
                {
                    OrderNumber = request.OrderNumber,
                    //Name = g.Name,
                    ProductId = g.ProductId,
                    Space = g.Space,
                    Weight = g.Weight
                }).ToList()
            };

            delivery = CalculateDelivery(delivery, request.ReadyToShipmentDate);
            if (delivery == null)
                return null;

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
                throw new Exception("The location is not found");
            }
            dbLocation.CurrentAddress = newAddress;
            // другие параметры могут пересчитаться в зависимости от адреса
            await _context.SaveChangesAsync();
            return dbLocation;
        }
    }
}
