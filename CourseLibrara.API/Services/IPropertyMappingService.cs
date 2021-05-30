using System.Collections.Generic;

namespace CourseLibrara.API.Services
{
    public interface IPropertyMappingService
    {
        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
    }
}