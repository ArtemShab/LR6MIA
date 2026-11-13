using AutoMapper;
using DryCleaningApi.DTOs;
using DryCleaningApi.Models;
namespace DryCleaningApi.Mappings;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateCleanClientDto, CleanClient>();
        CreateMap<UpdateCleanClientDto, CleanClient>();
        CreateMap<CleanClient, CleanClientResponseDto>();
        CreateMap<CreateGarmentItemDto, GarmentItem>();
        CreateMap<UpdateGarmentItemDto, GarmentItem>();
        CreateMap<GarmentItem, GarmentItemResponseDto>();
        CreateMap<CleaningReceipt, CleaningReceiptResponseDto>();
    }
}