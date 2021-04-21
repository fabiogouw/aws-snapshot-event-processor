using LoansEventProcessor.Core.Application.Ports.In;
using LoansEventProcessor.Core.Domain;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using LoansEventProcessor.Core.Application.Ports.Out;

namespace LoansEventProcessor.Core.Application
{
    public class SnapshotGenerationUseCase
    {
        private readonly Dictionary<string, IEventProcessor> _processors = new Dictionary<string, IEventProcessor>()
        {
            ["LoanCreated_v1"] = new LoanCreatedEventProcessor()
        };

        private readonly IEventRepository _eventRepository;
        private readonly ILoanRepository _loanRepository;

        public SnapshotGenerationUseCase(IEventRepository eventRepository, ILoanRepository loanRepository)
        {
            _eventRepository = eventRepository;
            _loanRepository = loanRepository;
        }

        public void Process(Event newEvent)
        {
            // 1.1. read snapshot
            var loanSnapshot = _loanRepository.GetLoan(newEvent.EntityId);
            // 1.2. add +1 to version
            newEvent.Version = loanSnapshot.LastEventVersion + 1;
            // 1.3. conditional insert
            _eventRepository.SaveEvent(newEvent);
            // 2.2. read all events since snapshot's version
            List<Event> events = new List<Event>();
            // 2.3. process events to generate new snapshot
            foreach (var @event in events.OrderBy(e => e.Version))
            {
                var processor = GetEventProcessor(@event.EventType);
                loanSnapshot.Item = processor.ProcessEvent(@event, loanSnapshot.Item);
            }
            // 2.4. update snapshot last event version
            loanSnapshot.LastEventVersion = newEvent.Version;
            // 2.5. conditional update
            _loanRepository.SaveLoan(loanSnapshot);

            // 3.1 generate "thin" vision
            // 3.2 conditional update (considering snapshot's last event version)
        }

        private IEventProcessor GetEventProcessor(string eventType)
        {
            return _processors[eventType];
        }
    }
}
