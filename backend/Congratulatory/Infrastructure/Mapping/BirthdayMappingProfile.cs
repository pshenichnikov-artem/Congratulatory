using AutoMapper;
using CoreService.Core.DTOs.Birthday;
using CoreService.Core.Entities;

namespace CoreService.Infrastructure.Mapping;

public class BirthdayMappingProfile : Profile
{
    public BirthdayMappingProfile()
    {
        CreateMap<Birthday, BirthdayResponse>()
             .ForMember(dest => dest.Notifications, opt => opt.MapFrom(src => src.BirthdayNotifications));
        
        CreateMap<BirthdayCreateRequest, Birthday>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.PhotoPath, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName.Trim()))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.Date));
        
        CreateMap<BirthdayUpdateRequest, Birthday>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.PhotoPath, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName.Trim()))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.Date));
    }
}