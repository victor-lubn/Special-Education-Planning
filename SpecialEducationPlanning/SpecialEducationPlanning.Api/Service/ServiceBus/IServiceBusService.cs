using System.Threading.Tasks;

namespace SpecialEducationPlanning
.Api.Service.ServiceBus
{
    public interface IServiceBusService
    {
        Task SendMessageAsync<T>(T messageBody, string queueOrTopicName) where T : class;
    }
}
