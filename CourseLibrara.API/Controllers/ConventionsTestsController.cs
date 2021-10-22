using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CourseLibrara.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // We can also apply these conventions at controller level
    // The conventions applied at method level overrides the one applied at controller level.
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class ConventionsTestsController : ControllerBase
    {
        // GET: api/<ConventionsTestsController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ConventionsTestsController>/5
        [HttpGet("{id}")]
        // To apply the conventions, we use the APiConventionMethod attribute, and we pass through the type of the convention we want to apply.
        // DefaultApiConventions are built-in conventions.
        // Attributes override conventions even when the attributes are applied at controller level or globally and the conventions
        // are applied at the method level
        // To this work, comment out our global filter
        //[ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ConventionsTestsController>
        [HttpPost]
        [ApiConventionMethod(typeof(CustomConventions), nameof(CustomConventions.Insert))]
        public void Insert([FromBody] string value)
        {
        }

        // PUT api/<ConventionsTestsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ConventionsTestsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
