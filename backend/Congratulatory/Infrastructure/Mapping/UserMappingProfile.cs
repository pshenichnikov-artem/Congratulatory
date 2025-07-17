using AutoMapper;
using CoreService.Core.DTOs.BirthdayNotification;
using CoreService.Core.DTOs.User;
using CoreService.Core.DTOs.UserAccount;
using CoreService.Core.Entities;

namespace CoreService.Infrastructure.Mapping;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<ApplicationUser, UserResponse>()
            .ForMember(dest => dest.UserAccounts, opt => opt.MapFrom(src => src.UserAccounts));
        
        CreateMap<UserAccount, UserAccountResponse>();
        CreateMap<UserAccountRequest, UserAccount>();

        CreateMap<BirthdayNotification, BirthdayNotificationResponse>()
            .ForMember(dest => dest.NotificationDate, opt => opt.MapFrom(src => src.NotificationDate.ToString("MM-dd HH:mm")));
        CreateMap<BirthdayNotificationRequest, BirthdayNotification>();
    }
}