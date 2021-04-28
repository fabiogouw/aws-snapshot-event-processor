using LoansEventProcessor.Core.Application.Ports.Input;
using LoansEventProcessor.Core.Domain;
using Microsoft.Extensions.Logging;

namespace LoansEventProcessor.Core.Application
{
    public class DummyEventProcessor : IEventProcessor
    {
        private readonly ILogger _logger;
        public DummyEventProcessor(ILogger logger)
        {
            _logger = logger;
        }

        public Loan ProcessEvent(Event @event, Loan current)
        {
            _logger.LogWarning($"{ @event.Id } - { @event.Content }");
            return current;
        }
    }
}
