using System;
using System.Collections.Generic;
using System.Text;

namespace LoansEventProcessor.Core.Application.Ports.Output
{
    public class Snapshot<T> where T : class
    {
        public Snapshot(T item, int lastEventVersion)
        {
            Item = item;
            LastEventVersion = lastEventVersion;
        }
        public T Item { get; set; }
        public int LastEventVersion { get; set; }
        public int NewEventVersion { get; set; }
    }
}
