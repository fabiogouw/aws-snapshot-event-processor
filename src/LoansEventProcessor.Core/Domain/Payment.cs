using System;
using System.Collections.Generic;
using System.Text;

namespace LoansEventProcessor.Core.Domain
{
    class Payment
    {
        private Installment _installment;
        public Payment(Installment installment)
        {
            _installment = installment;
        }

        public string Id { get; set; }
        public DateTime PaidDate { get; set; }
    }
}
