using AutoMapper;
using CourseLibrara.API.Helpers;
using CourseLibrara.API.Models;
using CourseLibrara.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrara.API.Controllers
{
    [ApiController]
    [Route("api/authorcollections")]
    public class AuthorCollectionsController : ControllerBase
    {
        private readonly ICourseLibraryRepository courseLibraryRepository;
        private readonly IMapper mapper;

        public AuthorCollectionsController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
        {
            this.courseLibraryRepository = courseLibraryRepository ?? throw new ArgumentNullException(nameof(courseLibraryRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // array key: 1,2,3
        // composite key: key1=value1, key2=value2, ...

        [HttpGet("({ids})", Name = "GetAuthorCollection")]
        public IActionResult GetAuthorCollection([FromRoute][ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if(ids == null)
            {
                return BadRequest();
            }

            var authorEntities = courseLibraryRepository.GetAuthors(ids);

            if(ids.Count() != authorEntities.Count())
            {
                return NotFound();
            }

            var authorsToReturn = mapper.Map<IEnumerable<AuthorDto>>(authorEntities);

            return Ok(authorsToReturn);
        }

        [HttpPost]
        public ActionResult<IEnumerable<AuthorDto>> CreateAuthorCollection(IEnumerable<AuthorForCreationDto> authorCollection)
        {
            var authorEntities = mapper.Map<IEnumerable<Entities.Author>>(authorCollection);
            foreach(var author in authorEntities)
            {
                courseLibraryRepository.AddAuthor(author);
            }

            courseLibraryRepository.Save();

            var authorCollectionToReturn = mapper.Map<IEnumerable<AuthorDto>>(authorEntities);
            var idsAsString = string.Join(",", authorCollectionToReturn.Select(a => a.Id));
            return CreatedAtRoute("GetAuthorCollection",
                new { ids = idsAsString }, authorCollectionToReturn);
        }
    }
}
