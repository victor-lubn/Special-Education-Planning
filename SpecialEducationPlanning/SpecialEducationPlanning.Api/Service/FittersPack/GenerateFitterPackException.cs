using System;

namespace SpecialEducationPlanning
.Api.Service.FittersPack
{
    [Serializable]
    internal class GenerateFitterPackException : Exception
    {
        public GenerateFitterPackException()
        {
        }

        public GenerateFitterPackException(string message) : base(message)
        {
        }

        public GenerateFitterPackException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}