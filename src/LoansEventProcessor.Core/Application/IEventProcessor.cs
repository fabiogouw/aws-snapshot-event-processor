using LoansEventProcessor.Core.Application.Ports.In;
using LoansEventProcessor.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoansEventProcessor.Core.Application
{
    public interface IEventProcessor
    {
        Loan ProcessEvent(Event @event, Loan current);
    }
}
