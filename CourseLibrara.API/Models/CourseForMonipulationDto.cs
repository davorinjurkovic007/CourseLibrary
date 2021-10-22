using CourseLibrara.API.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrara.API.Models
{
    /// <summary>
    /// Manipulation with courses
    /// </summary>
    [CourseTitleMustBeDifferentFromDescription(ErrorMessage = "Title must be different from description.")]
    public abstract class CourseForMonipulationDto
    {
        /// <summary>
        /// Title of the course
        /// </summary>
        [Required(ErrorMessage = "You should fill out a title.")]
        [MaxLength(100, ErrorMessage = "The title shouldn't have more than 100 characters.")]
        public string Title { get; set; }

        /// <summary>
        /// The description of the course
        /// </summary>
        [MaxLength(1500, ErrorMessage = "The description shouldn't have more tha 1500 characters.")]
        public virtual string Description { get; set; }

    }
}
