using System;
using System.Collections.Generic;
using System.Text;

namespace LoansEventProcessor.Core.Application.Ports.In
{
    public class Event
    {
        public Event(string id, string entityId, string eventType, DateTimeOffset eventTime, string content)
        {
            Id = id;
            EntityId = entityId;
            EventType = eventType;
            EventTime = eventTime;
            Content = content;
        }
        public string Id { get; private set; }
        public string EntityId { get; private set; }
        public string EventType { get; private set; }
        public DateTimeOffset EventTime { get; private set; }
        public int Version { get; set; }
        public string Content { get; private set; }
    }
}
