﻿using AutoMapper;
using OTUS.HomeWork.Clients;
using OTUS.HomeWork.EShop.Domain;
using OTUS.HomeWork.EShop.Domain.DTO;
using OTUS.HomeWork.RestAPI.Abstraction.Domain;
using System.Linq;

namespace OTUS.HomeWork.EShop
{
    public class AutoMapperProfile
        : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CreateOrderDTO, Order>()
                .ForMember(g => g.PaidDateUtc, m => m.Ignore())
                .ForMember(g => g.TotalPrice, m => m.Ignore())
                .ForMember(g => g.BillingId, m => m.Ignore())
                .ForMember(g => g.PaidDateUtc, m => m.Ignore())
                .ForMember(g => g.CreatedOnUtc, m => m.Ignore())
                .ForMember(g => g.OrderNumber, m => m.Ignore())
                .ForMember(g => g.UserId, m => m.Ignore())
                .ForMember(g => g.Status, m => m.Ignore())
                .ForMember(g => g.ErrorDescription, m => m.Ignore())
                .ForMember(g => g.Items, m => m.Ignore())
                .ReverseMap();

            CreateMap<OrderItemDTO, OrderItem>()
                .ForMember(g => g.Order, m => m.Ignore())
                .ForMember(g => g.OrderNumberId, m => m.Ignore())
                .ReverseMap();

            CreateMap<Order, OrderDTO>();

            CreateMap<RegisterUserDTO, User>()
               .ForMember(g => g.Id, m => m.Ignore());

            CreateMap<User, UserDTO>()
                .ForMember(g => g.UserId,
                    m => m.MapFrom(s => s.Id))
                .ReverseMap();


            CreateMap<BucketRequestDTO, Bucket>()
                .ForMember(g => g.Items, m => m.MapFrom(s => s.Items))
                .ForMember(g => g.UserId, m => m.Ignore());

            CreateMap<OrderItemDTO, BucketItem>()
                .ForMember(g => g.Bucket, m => m.Ignore())
                .ForMember(g => g.BucketId, m => m.Ignore());

            CreateMap<Bucket, BucketResponseDTO>()
                .ForMember(g => g.SummaryPrice, m => m.Ignore())
                .ForMember(g => g.Discount, m => m.Ignore());

            CreateMap<BucketItem, BucketItemDTO>();

            CreateMap<DeliveryLocationDTO, OrderLocationDTO>()
                .ForMember(g => g.DeliveryDate, m => m.MapFrom(s => s.DeliveryDate.UtcDateTime));
        }
    }
}
