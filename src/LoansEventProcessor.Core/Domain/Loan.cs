using System;
using System.Collections.Generic;
using System.Text;

namespace LoansEventProcessor.Core.Domain
{
    public class Loan
    {
        public string Id { get; set; }
        public int Version { get; set; }
        public DateTime ContractDate { get; set; }
        public decimal ContractAmount { get; set; }
        public List<Installment> Installments { get; private set; }
    }
}
