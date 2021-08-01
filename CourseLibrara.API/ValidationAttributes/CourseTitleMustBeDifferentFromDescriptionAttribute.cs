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
    /// </summary>
    public class CourseTitleMustBeDifferentFromDescriptionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var course = (CourseForCreationDto)validationContext.ObjectInstance;

            if (course.Title == course.Description)
            {
                return new ValidationResult(
                    ErrorMessage,
                    new[] { nameof(CourseForCreationDto) });
            }

            return ValidationResult.Success;
        }

    }
}
