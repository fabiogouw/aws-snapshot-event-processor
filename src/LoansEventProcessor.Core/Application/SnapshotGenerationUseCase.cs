using LoansEventProcessor.Core.Application.Ports.Input;
using LoansEventProcessor.Core.Application.Ports.Output;
using LoansEventProcessor.Core.Domain;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LoansEventProcessor.Core.Application
{
    public class SnapshotGenerationUseCase
    {
        private readonly Dictionary<string, IEventProcessor> _processors;
        private readonly ILoanRepository _loanRepository;
        private readonly ILogger _logger;

        public SnapshotGenerationUseCase(ILogger logger, ILoanRepository loanRepository)
        {
            _logger = logger;
            _loanRepository = loanRepository;
            _processors = new Dictionary<string, IEventProcessor>()
            {
                ["LoanCreated.v1"] = new LoanCreatedEventProcessor(),
                ["dummy"] = new DummyEventProcessor(logger)
            };
        }

        public async Task Process(Event newEvent)
        {
            var loanSnapshot = await _loanRepository.GetLoan(newEvent.LoanId)
                ?? new Snapshot<Loan>(new Loan(newEvent.LoanId), newEvent.LoanId);

            var processor = GetEventProcessor(newEvent.EventType);
            var loan = processor.ProcessEvent(newEvent, loanSnapshot.Item);

            loanSnapshot.SetNewSnapshotVersion(loan, newEvent);
            
            await _loanRepository.SaveLoan(loanSnapshot);

            // generate "thin" vision
            // conditional update (considering snapshot's last event version)
        }

        private IEventProcessor GetEventProcessor(string eventType)
        {
            return _processors[eventType];
        }
    }
}
