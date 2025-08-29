using Microsoft.Extensions.Logging;
using Moq;
using System;

namespace SpecialEducationPlanning
.Api.Test.Support
{
    public static class MockLoggerHelper
    {
        public static void MockLog<T>(this Mock<ILogger<T>> logger, LogLevel logLevel, string message = null)
        {
            logger.Setup(x => x.Log(
                logLevel,
                It.IsAny<EventId>(),
                string.IsNullOrWhiteSpace(message) ? It.IsAny<object>() : message,
                It.IsAny<Exception>(),
                It.IsAny<Func<object, Exception, string>>()
                )).Verifiable();
        }

        public static void VerifyLogger<T>(this Mock<ILogger<T>> logger, Times times, LogLevel expectedLogLevel)
        {
            logger.Verify(x => x.Log(
                It.Is<LogLevel>(l => l == expectedLogLevel),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((value, type) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((value, type) => true)), times);
        }

        public static void VerifyLogger<T>(this Mock<ILogger<T>> logger, LogLevel expectedLogLevel, string message, Times? times = null)
        {
            times ??= Times.Once();
            Func<object, Type, bool> areequal = (v, t) => v.ToString().CompareTo(message) == 0;

            logger.Verify(x => x.Log(
                    It.Is<LogLevel>(l => l == expectedLogLevel),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => areequal(v, t)),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), (Times)times);
        }
    }
}
