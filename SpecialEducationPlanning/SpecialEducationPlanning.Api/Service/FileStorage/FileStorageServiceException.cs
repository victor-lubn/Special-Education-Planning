﻿using System;
using System.Runtime.Serialization;

namespace SpecialEducationPlanning
.Api.Service.FileStorage
{
    [Serializable]
    public class FileStorageServiceException : Exception
    {
        public FileStorageServiceException()
        {
        }

        public FileStorageServiceException(string message) : base(message)
        {
        }

        public FileStorageServiceException(string message, Exception inner) : base(message, inner)
        {
        }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected FileStorageServiceException(SerializationInfo info,
            StreamingContext context)
        {
        }
    }
}