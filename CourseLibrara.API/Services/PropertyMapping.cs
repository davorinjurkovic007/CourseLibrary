using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrara.API.Services
{
    public class PropertyMapping<TSource, TDestination> : IPropertyMapping
    {
        public Dictionary<string, PropertyMappingValue> mappingDictionary { get; set; }

        public PropertyMapping(Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            this.mappingDictionary = mappingDictionary ?? throw new ArgumentNullException(nameof(mappingDictionary));
        }
    }
}
