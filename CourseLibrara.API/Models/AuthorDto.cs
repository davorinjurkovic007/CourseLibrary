using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrara.API.Models
{
    /// <summary>
    /// An author with Id, Name, Age and Main category
    /// </summary>
    public class AuthorDto
    {
        /// <summary>
        /// The Id of the author
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The name of the author
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///  The age of the author
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// The main category of the author
        /// </summary>
        public string MainCategory { get; set; }
    }
}
