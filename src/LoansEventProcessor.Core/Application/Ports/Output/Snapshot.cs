using LoansEventProcessor.Core.Application.Ports.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoansEventProcessor.Core.Application.Ports.Output
{
    public class Snapshot<T> where T : class
    {
        public Snapshot(T item, string id, int lastEventVersion)
        {
            Item = item;
            Id = id;
            LastEventVersion = lastEventVersion;
        }
        public Snapshot(T item, string id) :
            this(item, id, 0)
        {
        }
        public string Id { get; private set; }
        public T Item { get; private set; }
        public int LastEventVersion { get; private set; }
        public int NewEventVersion { get; private set; }
        public DateTimeOffset UpdatedAt { get; private set; }
        public Event Event { get; private set; }

        public void SetNewSnapshotVersion(T item, Event @event)
        {
            Item = item;
            UpdatedAt = DateTimeOffset.UtcNow;
            NewEventVersion = LastEventVersion + 1;
            Event = @event;
            @event.ProcessesAt = UpdatedAt;
        }
    }
}
