using System;
using System.Collections.Generic;
using System.Text;

namespace LoansEventProcessor.Core.Domain
{
    public class Installment
    {
        public string Id { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
    }
}
