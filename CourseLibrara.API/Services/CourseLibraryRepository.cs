using CourseLibrara.API.DbContexts;
using CourseLibrara.API.Entities;
using CourseLibrara.API.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrara.API.Services
{
    public class CourseLibraryRepository : ICourseLibraryRepository, IDisposable
    {
        private readonly CourseLibraryContext context;

        public CourseLibraryRepository(CourseLibraryContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void AddAuthor(Author author)
        {
            if(author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            // the repository fills the id (instead of using indentity columns)
            author.Id = Guid.NewGuid();

            foreach(var course in author.Courses)
            {
                course.Id = Guid.NewGuid();
            }

            context.Authors.Add(author);
        }

        public void AddCourse(Guid authorId, Course course)
        {
            if(authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            if(course == null)
            {
                throw new ArgumentNullException(nameof(course));
            }
            // always set the AuthorId to the passed-in authorId
            course.AuthorId = authorId;
            context.Courses.Add(course);
        }

        public bool AuthorExists(Guid authorId)
        {
            if(authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return context.Authors.Any(a => a.Id == authorId);
        }

        public void DeleteAuthor(Author author)
        {
            if(author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            context.Authors.Remove(author);
        }

        public void DeleteCourse(Course course)
        {
            context.Courses.Remove(course);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Author GetAuthor(Guid authorId)
        {
            if(authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return context.Authors.FirstOrDefault(a => a.Id == authorId);
        }

        public IEnumerable<Author> GetAuthors()
        {
            return context.Authors.ToList<Author>();
        }

        public IEnumerable<Author> GetAuthors(AuthorsResourceParameters authorsResourceParameters)
        {
            if(authorsResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(authorsResourceParameters));
            }

            if(string.IsNullOrWhiteSpace(authorsResourceParameters.MainCategory) && string.IsNullOrWhiteSpace(authorsResourceParameters.SearchQuery))
            {
                GetAuthors();
            }

            var collection = context.Authors as IQueryable<Author>;

            if(!string.IsNullOrWhiteSpace(authorsResourceParameters.MainCategory))
            {
                var mainCategory = authorsResourceParameters.MainCategory.Trim();
                collection = collection.Where(a => a.MainCategory == mainCategory);
            }

            if(!string.IsNullOrWhiteSpace(authorsResourceParameters.SearchQuery))
            {
                var searchQuery = authorsResourceParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.MainCategory.Contains(searchQuery)
                || a.FirstName.Contains(searchQuery)
                || a.LastName.Contains(searchQuery));
            }

            return collection.ToList();
        }

        public IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds)
        {
            if(authorIds == null)
            {
                throw new ArgumentNullException(nameof(authorIds));
            }

            return context.Authors.Where(a => authorIds.Contains(a.Id)).OrderBy(a => a.FirstName).OrderBy(a => a.LastName).ToList();
        }

        public Course GetCourse(Guid authorId, Guid courseId)
        {
            if(authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            if(courseId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(courseId));
            }

            return context.Courses.Where(c => c.AuthorId == authorId && c.Id == courseId).FirstOrDefault();
        }

        public IEnumerable<Course> GetCourses(Guid authorId)
        {
            if(authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return context.Courses.Where(c => c.AuthorId == authorId).OrderBy(c => c.Title).ToList();
        }

        public bool Save()
        {
            return (context.SaveChanges() >= 0);
        }

        public void UpdateAuthor(Author author)
        {
            // no code in this implementation
        }

        public void UpdateCourse(Course course)
        {
            // not code in this implementation
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                // dispose resource when needed
            }
        }
    }
}
