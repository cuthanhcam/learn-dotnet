using DevForge.Domain.Specifications;

namespace DevForge.Application.Common.Extensions
{
    /// <summary>
    /// Extension methods for Specifications
    /// </summary>
    public static class SpecificationExtensions
    {
        /// <summary>
        /// Combines multiple specifications with AND logic
        /// </summary>
        public static Specification<T> AndAll<T>(this IEnumerable<Specification<T>> specifications)
        {
            var specList = specifications.ToList();
            if (!specList.Any())
                throw new ArgumentException("At least one specification is required");

            var combined = specList.First();
            foreach (var spec in specList.Skip(1))
            {
                combined = combined.And(spec);
            }

            return combined;
        }

        /// <summary>
        /// Combines multiple specifications with OR logic
        /// </summary>
        public static Specification<T> OrAll<T>(this IEnumerable<Specification<T>> specifications)
        {
            var specList = specifications.ToList();
            if (!specList.Any())
                throw new ArgumentException("At least one specification is required");

            var combined = specList.First();
            foreach (var spec in specList.Skip(1))
            {
                combined = combined.Or(spec);
            }

            return combined;
        }
    }
}
