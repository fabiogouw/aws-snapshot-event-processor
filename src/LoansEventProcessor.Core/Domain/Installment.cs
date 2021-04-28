using System;
using System.Collections.Generic;
using System.Text;

namespace LoansEventProcessor.Core.Domain
{
    public class Installment
    {
        public Installment()
        {

        }

        public Installment(string id, DateTime dueDate, decimal amount)
        {

        }
        public string Id { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public DateTime? PaidDate { get; set; }
        public DateTime? ReversalDate { get; set; }
        public string Status 
        {
            get
            {
                if (!PaidDate.HasValue)
                {
                    if(!ReversalDate.HasValue)
                    {
                        return "Payment Reversed";
                    }
                    return "Paid";
                }
                return "Pending";
            }
        }
    }
}
