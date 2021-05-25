using AutoMapper;
using CourseLibrara.API.Models;
using CourseLibrara.API.ResourceParameters;
using CourseLibrara.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CourseLibrara.API.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController : ControllerBase
    {
        private readonly ICourseLibraryRepository courseLibraryRepository;
        private readonly IMapper mapper;

        public AuthorsController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
        {
            this.courseLibraryRepository = courseLibraryRepository ?? throw new ArgumentNullException(nameof(courseLibraryRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet()]
        [HttpHead]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthors([FromQuery] AuthorsResourceParameters authorsResourceParameters)
        {
            var authorsFromRepo = courseLibraryRepository.GetAuthors(authorsResourceParameters);

            return Ok(mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo));
        }

        [HttpGet("{authorId:guid}", Name = "GetAuthor")]
        public IActionResult GetAuthor(Guid authorId)
        {
            var authorFromRepo = courseLibraryRepository.GetAuthor(authorId);

            if(authorFromRepo == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<AuthorDto>(authorFromRepo));
        }

        [HttpPost]
        public ActionResult<AuthorDto> CreateAuthor(AuthorForCreationDto author)
        {
            var authorEntity = mapper.Map<Entities.Author>(author);
            courseLibraryRepository.AddAuthor(authorEntity);
            courseLibraryRepository.Save();

            var authorToReturn = mapper.Map<AuthorDto>(authorEntity);
            return CreatedAtRoute("GetAuthor",
                new { authorId = authorToReturn.Id },
                authorToReturn);
        }

        [HttpOptions]
        public IActionResult GetAuthorsOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok();
        }

        [HttpDelete("{authorId}")]
        public ActionResult DeleteAuthor(Guid authorId)
        {
            var authorFromRepo = courseLibraryRepository.GetAuthor(authorId);

            if(authorFromRepo == null)
            {
                return NotFound();
            }

            courseLibraryRepository.DeleteAuthor(authorFromRepo);

            courseLibraryRepository.Save();

            return NoContent();
        }
    }
}
