using System;

namespace SpecialEducationPlanning
.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class DefineHeaderAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }
        public bool IsRequired { get; }

        public DefineHeaderAttribute(string name, string description = null, bool isRequired = false)
        {
            Name = name;
            Description = description;
            IsRequired = isRequired;
        }
    }
}

