using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrara.API.Models
{
    public class CourseForUpdateDto : CourseForMonipulationDto
    {
        [Required(ErrorMessage = "You shold fill out a description.")]
        public override string Description { get => base.Description; set => base.Description = value; }
    }
}
