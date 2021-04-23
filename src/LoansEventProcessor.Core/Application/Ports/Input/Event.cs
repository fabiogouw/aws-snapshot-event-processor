using System;
using System.Collections.Generic;
using System.Text;

namespace LoansEventProcessor.Core.Application.Ports.Input
{
    public class Event
    {
        public Event(string entityId, string id, string eventType, long eventTimeInSeconds, string content)
        {
            EntityId = entityId;
            Id = id;
            EventType = eventType;
            EventTime = DateTimeOffset.FromUnixTimeSeconds(eventTimeInSeconds);
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
