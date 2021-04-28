using System;
using System.Collections.Generic;
using System.Text;

namespace LoansEventProcessor.Core.Application.Ports.Input
{
    public class Event
    {
        public Event(string loanId, string id, string eventType, long eventTimeInMilliseconds, string content)
        {
            LoanId = loanId;
            Id = id;
            EventType = eventType;
            EventTime = DateTimeOffset.FromUnixTimeMilliseconds(eventTimeInMilliseconds);
            Content = content;
        }
        public string Id { get; private set; }
        public string LoanId { get; private set; }
        public string EventType { get; private set; }
        public DateTimeOffset EventTime { get; private set; }
        public DateTimeOffset ProcessesAt { get; set; }
        public string Content { get; private set; }
    }
}
