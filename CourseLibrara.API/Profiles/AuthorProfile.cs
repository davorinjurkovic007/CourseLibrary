using AutoMapper;
using CourseLibrara.API.Helpers;

namespace CourseLibrara.API.Profiles
{
    public class AuthorProfile : Profile
    {
        public AuthorProfile()
        {
            CreateMap<Entities.Author, Models.AuthorDto>()
                .ForMember(
                    dest => dest.Name,
                    opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(
                    dest => dest.Age,
                    opt => opt.MapFrom(src => src.DateOfBirth.GetCurrentAge(src.DateOfDeath)));

            CreateMap<Models.AuthorForCreationDto, Entities.Author>();

            CreateMap<Models.AuthorForCreationWithDateOfDeathDto, Entities.Author>();

            CreateMap<Entities.Author, Models.AuthorFullDto>();
        }
    }
}
