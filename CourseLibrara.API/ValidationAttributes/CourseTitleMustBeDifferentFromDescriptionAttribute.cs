using CourseLibrara.API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrara.API.ValidationAttributes
{
    /// <summary>
    /// NOTE: The problem is this request: POST Author with Courses
    /// This is where is happen error. 
    /// This is just for demonstration to use with this requests:
    /// POST Course for Author (null values)
    /// POST Course for Author (title == description)
    /// POST Course for Author (long title == long description)
    /// </summary>
    public class CourseTitleMustBeDifferentFromDescriptionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var course = (CourseForMonipulationDto)validationContext.ObjectInstance;

            if (course.Title == course.Description)
            {
                return new ValidationResult(
                    ErrorMessage,
                    new[] { nameof(CourseForMonipulationDto) });
            }

            return ValidationResult.Success;
        }

    }
}
