using System.Collections.Generic;

namespace SpecialEducationPlanning
.Business.Mapper
{
    /// <summary>
    ///     Object mapper interface
    /// </summary>
    public interface IObjectMapper
    {
        /// <summary>
        ///     Maps the given source entity to new target
        /// </summary>
        /// <typeparam name="S">Source type</typeparam>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="source">Object to map from</param>
        /// <returns>New object instance mapped</returns>
        T Map<S, T>(S source);

        /// <summary>
        ///     Maps the given source entity to existing target instance
        /// </summary>
        /// <typeparam name="S">Source type</typeparam>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="source">Object to map from</param>
        /// <param name="target">object to map to</param>
        /// <returns>target object mapped</returns>
        T Map<S, T>(S source, T target);

        /// <summary>
        ///     Maps a collection of source items to new targets
        /// </summary>
        /// <typeparam name="S">Source type</typeparam>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="source">Source collection of objets</param>
        /// <returns>A mapped collection</returns>
        IEnumerable<T> Map<S, T>(IEnumerable<S> source);
    }
}