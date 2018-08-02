namespace DatingApp.API.Helpers
{
    using System.Linq;
    using AutoMapper;
    using DatingApp.API.Dtos;
    using DatingApp.API.Models;

    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserListDto>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.ResolveUsing(d => d.DateOfBirth.CalculateAge()));
            CreateMap<User, UserDetailsDto>()
            
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.ResolveUsing(d => d.DateOfBirth.CalculateAge()));

            CreateMap<Photo, PhotoDetailsDto>();
            CreateMap<UserEditDto, User>();
            CreateMap<Photo, PhotoReturnDto>();
            CreateMap<PhotoCreationDto, Photo>();
            CreateMap<UserRegisterDto, User>();

            CreateMap<MessageCreateDto, Message>()
                .ReverseMap();

            CreateMap<Message, MessageReturnDto>()
                .ForMember(m => m.SenderPhotoUrl, opt => opt.MapFrom(u => u.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(m => m.RecipientPhotoUrl, opt => opt.MapFrom(u => u.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url));
        }
    }
}