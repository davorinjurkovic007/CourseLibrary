using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrara.API.Profiles
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<Entities.Course, Models.CourseDto>();
            CreateMap<Models.CourseForCreationDto, Entities.Course>();
            CreateMap<Models.CourseForUpdateDto, Entities.Course>();
            CreateMap<Entities.Course, Models.CourseForUpdateDto>();
        }
    }
}
