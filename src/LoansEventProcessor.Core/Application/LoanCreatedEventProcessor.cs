using LoansEventProcessor.Core.Application.Ports.In;
using LoansEventProcessor.Core.Domain;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoansEventProcessor.Core.Application
{
    public class LoanCreatedEventProcessor : IEventProcessor
    {
        public Loan ProcessEvent(Event @event, Loan current)
        {
            JObject json = JObject.Parse(@event.Content);
            
            throw new NotImplementedException();
        }
    }
}
