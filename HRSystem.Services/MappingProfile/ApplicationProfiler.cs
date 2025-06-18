using AutoMapper;
using POSSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSSystem.Services.MappingProfile
{
    public class ApplicationProfiler : Profile
    {
        public ApplicationProfiler()
        {
            CreateMap<AddItemDto, Item>();
            CreateMap<Item, GetItemDto>().ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category.Name));

            CreateMap<InvoiceDetailedDto, Invoice>().ReverseMap();
            CreateMap<InvoiceCreateDto, Invoice>().ForMember(dest => dest.InvoiceDetails,
                opt => opt.MapFrom(src => src.Items));

            CreateMap<InvoiceUpdateDto, Invoice>();
            CreateMap<Invoice, InvoiceListDto>();
            CreateMap<InvoiceDetailCreateDto, InvoiceDetail>();
            CreateMap<InvoiceDetailUpdateDto, InvoiceDetail>();
            CreateMap<InvoiceDetail, InvoiceDetailItemDto>()
                .ForMember(dest => dest.ItemName,
                opt => opt.MapFrom(src => src.Item.Name));



        }
    }
}
