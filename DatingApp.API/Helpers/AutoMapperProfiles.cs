using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using System.Linq;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.PhotoUrl, opt =>
                {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(f => f.IsMain).Url);
                })
                .ForMember(dest => dest.Age, opt =>
                {
                    opt.MapFrom(src => src.DateOfBirth.CalculateAge());
                });

            CreateMap<User, UserForDetailsDto>().ForMember(dest => dest.PhotoUrl, opt =>
            {
                opt.MapFrom(src => src.Photos.FirstOrDefault(f => f.IsMain).Url);
            })
                           .ForMember(dest => dest.Age, opt =>
                           {
                               opt.MapFrom(src => src.DateOfBirth.CalculateAge());
                           });


            CreateMap<Photo, PhotosForDetailedDto>();
            CreateMap<Photo, PhotoToReturnDto>();
            CreateMap<PhotoForCreationDto, Photo>();
            CreateMap<UserForUpdateDto, User>();
            CreateMap<UserForRegisterDto, User>();
            CreateMap<MessageForCreationDto, Message>().ReverseMap();
            CreateMap<Message, MessageToReturnDto>()
                .ForMember(dest => dest.SenderPhotoUrl, opt =>
              {
                  opt.MapFrom(src => src.Sender.Photos.FirstOrDefault(f => f.IsMain).Url);
              })
            .ForMember(dest => dest.RecipientPhotoUrl, opt =>
              {
                  opt.MapFrom(src => src.Recipient.Photos.FirstOrDefault(f => f.IsMain).Url);
              });
        }
    }
}