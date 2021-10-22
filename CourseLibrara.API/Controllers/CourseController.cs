using AutoMapper;
using CourseLibrara.API.Models;
using CourseLibrara.API.Services;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrara.API.Controllers
{
    // This attribute allows us to restrict a response format, signified by media types for a specific action, controller, or at global level.
    // Swashbuckle looks for this attribute and adjusts the generated spec accordingly.
    // Controller scope overrides global scope, and action scope overrides controller scope. So you can be very specific if needed.
    // This attribute to override any API conventions that might have been applied, another reason not to use them
    // Very important to know is that the Produces filter forces actions decorated with it, or in our case all action in oru controller, to return 
    // respones in one of the media types passed through and that can have some unexpected side effects.
    [Produces("application/json", "application/xml")]
    [ApiController]
    [Route("api/authors/{authorId}/courses")]
    //[ResponseCache(CacheProfileName = "240SecondsCacheProfile")]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public)]
    [HttpCacheValidation(MustRevalidate = true)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //[ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class CourseController : ControllerBase
    {
        private readonly ICourseLibraryRepository courseLibraryRepository;
        private readonly IMapper mapper;

        public CourseController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
        {
            this.courseLibraryRepository = courseLibraryRepository ?? throw new ArgumentNullException(nameof(courseLibraryRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Get all courses for given authorId
        /// </summary>
        /// <param name="authorId">The id of the author</param>
        /// <returns>All courses for given author</returns>
        /// <response code="200">Returns all authors courses</response>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseDto))]
        [HttpGet(Name = "GetCoursesForAuthor")]
        public ActionResult<IEnumerable<CourseDto>> GetCoursesForAuthor(Guid authorId)
        {
            if(!courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var coursesForAuthorRepo = courseLibraryRepository.GetCourses(authorId);
            return Ok(mapper.Map<IEnumerable<CourseDto>>(coursesForAuthorRepo));
        }

        [HttpGet("{courseId}", Name = "GetCourseForAuthor")]
        //[ResponseCache(Duration = 120)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1000)]
        [HttpCacheValidation(MustRevalidate = false)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<CourseDto> GetCourseForAuthor(Guid authorId, Guid courseId)
        {
            if(!courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseForAuthorFromRepo = courseLibraryRepository.GetCourse(authorId, courseId);

            if(courseForAuthorFromRepo == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<CourseDto>(courseForAuthorFromRepo));
        }

        [HttpPost(Name = "CreateCoursesForAuthor")]
        // Just as we can specify the media types for output wiht the Produces attribute, we can specify media types for input with 
        // the Consumes attribute.
        [Consumes("application/json")]
        public ActionResult<CourseDto> CreateCoursesForAuthor(Guid authorId, CourseForCreationDto course)
        {
            if(!courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseEntity = mapper.Map<Entities.Course>(course);
            courseLibraryRepository.AddCourse(authorId, courseEntity);
            courseLibraryRepository.Save();

            var courseToReturn = mapper.Map<CourseDto>(courseEntity);
            return CreatedAtRoute("GetCourseForAuthor", new { authorId = authorId, courseId = courseToReturn.Id });
        }

        [HttpPut("{courseId}")]
        public IActionResult UpdateCourseForAuthor(Guid authorId, Guid courseId, CourseForUpdateDto course)
        {
            if(!courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseForAuthorFromRepo = courseLibraryRepository.GetCourse(authorId, courseId);

            if(courseForAuthorFromRepo == null)
            {
                var courseToAdd = mapper.Map<Entities.Course>(course);
                courseToAdd.Id = courseId;

                courseLibraryRepository.AddCourse(authorId, courseToAdd);

                courseLibraryRepository.Save();

                var courseToReturn = mapper.Map<CourseDto>(courseToAdd);

                return CreatedAtRoute("GetCourseForAuthor",
                    new { authorId = authorId, courseId = courseToReturn.Id },
                    courseToReturn);
            }

            // map the entitiy to a CourseForUpdateDto
            // apply the update field values to that dto
            // map the CourseForUpdateDto back to entity
            mapper.Map(course, courseForAuthorFromRepo);

            courseLibraryRepository.UpdateCourse(courseForAuthorFromRepo);

            courseLibraryRepository.Save();
            return NoContent();
        }

        /// <summary>
        /// Partially Update Course for Author
        /// </summary>
        /// <param name="authorId">The id of the author you want to get</param>
        /// <param name="courseId">The id of the course you waht to get</param>
        /// <param name="pathcDocument">The set of operations to apply to the course</param>
        /// <returns>An ActionResult of type Course</returns>
        /// <remarks>
        /// Sample request this request (this request just update **description**)  
        ///     PATCH /authors/authorid/courses/courseid  
        ///     [   
        ///         {   
        ///             "op": "replace",   
        ///             "path": "/description",   
        ///             "value": "new description"   
        ///         }   
        ///     ]  
        /// </remarks>
        [HttpPatch("{courseId}")]
        public ActionResult PartiallyUpdateCurseForAuthor(Guid authorId, Guid courseId, JsonPatchDocument<CourseForUpdateDto> pathcDocument)
        {
            if(!courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseForAuthorFromRepo = courseLibraryRepository.GetCourse(authorId, courseId);

            if(courseForAuthorFromRepo == null)
            {
                var courseDto = new CourseForUpdateDto();
                pathcDocument.ApplyTo(courseDto, ModelState);

                if(!TryValidateModel(courseDto))
                {
                    return ValidationProblem(ModelState);
                }

                var courseToAdd = mapper.Map<Entities.Course>(courseDto);
                courseToAdd.Id = courseId;

                courseLibraryRepository.AddCourse(authorId, courseToAdd);
                courseLibraryRepository.Save();

                var courseToReturn = mapper.Map<CourseDto>(courseToAdd);

                return CreatedAtRoute("GetCourseForAuthor",
                    new { authorId = authorId, courseId = courseToReturn.Id });
            }

            var courseToPathc = mapper.Map<CourseForUpdateDto>(courseForAuthorFromRepo);
            // add validation
            pathcDocument.ApplyTo(courseToPathc, ModelState);

            if(!TryValidateModel(courseToPathc))
            {
                return ValidationProblem(ModelState);
            }

            mapper.Map(courseToPathc, courseForAuthorFromRepo);

            courseLibraryRepository.UpdateCourse(courseForAuthorFromRepo);

            courseLibraryRepository.Save();

            return NoContent();
        }

        [HttpDelete("{courseId}")]
        public ActionResult DeleteCourseForAuthor(Guid authorId, Guid courseId)
        {
            if(!courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseForAuthorFromRepo = courseLibraryRepository.GetCourse(authorId, courseId);

            if(courseForAuthorFromRepo == null)
            {
                return NotFound();
            }

            courseLibraryRepository.DeleteCourse(courseForAuthorFromRepo);
            courseLibraryRepository.Save();

            return NoContent();
        }

        public override ActionResult ValidationProblem([ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
    }
}
