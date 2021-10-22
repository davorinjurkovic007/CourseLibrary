using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrara.API
{
    // A convention is, in essence, a static type with methods that's capable of defining response types and naming requirements on actions.
    public static class CustomConventions
    {
        // To this method, we then apply the attributes we want to see applied to our Insert method. 
        // We can use metching attributes to define how the matching of method names or parameters happens.
        // For example, we want to match this convention to any method that starts with Insert, so InsertTest will be a match. 
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
        public static void Insert(
            [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any)]
            [ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)]
            object model)
        {

        }
    }
}
