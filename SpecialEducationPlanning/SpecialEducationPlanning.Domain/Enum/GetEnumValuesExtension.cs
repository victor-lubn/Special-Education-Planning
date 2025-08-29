namespace SpecialEducationPlanning
.Domain.Enum
{
    /// <summary>
    /// 
    /// </summary>
    public static class GetEnumValues
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Desired EnumType to get values from</typeparam>
        /// <returns></returns>
        public static T[] GetValues<T>()
        {
            return (T[])System.Enum.GetValues(typeof(T));
        }
    }
}
