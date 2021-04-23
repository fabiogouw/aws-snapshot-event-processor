using LoansEventProcessor.Core.Application.Ports.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LoansEventProcessor.Core.Application.Ports.Output
{
    public interface IEventRepository
    {
        Task<Event> GetEvent(string eventId);
        Task<List<Event>> GetEventsSinceVersion(string entityId, int version);
        Task SaveEvent(Event @event);
    }
}
