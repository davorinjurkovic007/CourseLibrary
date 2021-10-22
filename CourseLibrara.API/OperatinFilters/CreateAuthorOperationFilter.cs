using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrara.API.OperatinFilters
{
    public class CreateAuthorOperationFilter : IOperationFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation">Generated operation that will result in part of our specification</param>
        /// <param name="context">Contains information we can potentially use to manipulate our operation, like tha API description</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.OperationId != "CreateAuthorWithDateOfDeath")
            {
                return;
            }

            //operation.Responses[StatusCodes.Status201Created.ToString()].Content.Add(
            //    "application/vnd.marvin.authorforcreation+json",
            //    new OpenApiMediaType()
            //    {
            //        Schema = context.SchemaRepository.
            //    });
        }
    }
}
