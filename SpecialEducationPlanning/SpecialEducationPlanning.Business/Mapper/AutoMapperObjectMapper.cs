using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace SpecialEducationPlanning
.Business.Mapper
{
    /// <summary>
    ///     AutoMapper IObjectMapper implementation
    /// </summary>
    public class AutoMapperObjectMapper : IObjectMapper
    {

        private readonly IMapper mapper;

        /// <summary>
        /// Creates a new instance of <see cref="AutoMapperObjectMapper"/>
        /// </summary>
        /// <param name="mapper">AutoMapper IMapper</param>
        public AutoMapperObjectMapper(IMapper mapper)
        {
            this.mapper = mapper;
        }

        /// <summary>
        /// Maps properties from a <paramref name="source"/> to an existing <paramref name="target"/> object instance.
        /// </summary>
        /// <typeparam name="TSource">Source Type</typeparam>
        /// <typeparam name="TTarget">Target Type</typeparam>
        /// <param name="source">Source instance</param>
        /// <param name="target">Target instance</param>
        /// <returns>Existing <paramref name="target"/> instance with <paramref name="source"/> mapped into</returns>
        public TTarget Map<TSource, TTarget>(TSource source, TTarget target)
        {

            return this.mapper.Map(source, target);
        }

        /// <summary>
        /// Maps properties from <typeparamref name="TSource"/> to a new instance <typeparamref name="TTarget"/>
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TTarget">Target type</typeparam>
        /// <param name="source">Source instance</param>
        /// <returns>new instance of <typeparamref name="TTarget"/> with <typeparamref name="TSource"/> mapped into</returns>
        public TTarget Map<TSource, TTarget>(TSource source)
        {
            return this.mapper.Map<TSource, TTarget>(source);
        }

        /// <summary>
        /// Maps properties from a collection <typeparamref name="TSource"/> to a new collection instance <typeparamref name="TTarget"/>
        /// </summary>
        /// <typeparam name="TSource">Source collection type</typeparam>
        /// <typeparam name="TTarget">Target collection type</typeparam>
        /// <param name="source">Source collection</param>
        /// <returns>Target collection</returns>
        public IEnumerable<TTarget> Map<TSource, TTarget>(IEnumerable<TSource> source)
        {
            return source.Select(x => this.Map<TSource, TTarget>(x));
        }

        /// <summary>
        /// Maps properties from a <paramref name="source"/> object of type <paramref name="sourceType"/> to a <paramref name="target"/> object of type <paramref name="targetType"/>
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="target">Target object</param>
        /// <param name="sourceType">Source type</param>
        /// <param name="targetType">Target type</param>
        /// <returns><paramref name="source"/> mapped into <paramref name="target"/></returns>
        public object Map(object source, object target, Type sourceType, Type targetType)
        {
            return this.mapper.Map(source, target, sourceType, targetType);
        }
    }
}