using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace LoansEventProcessor.Core.Domain
{
    public class Loan
    {
        public Loan(string id)
        {
            Id = id;
            Instalments = new List<Installment>();
        }
        public Loan()
        {

        }
        public string Id { get; set; }
        public DateTime ContractDate { get; set; }
        public decimal ContractAmount { get; set; }
        public List<Installment> Instalments { get; private set; }
        public void AddInstallment(DateTime dueDate, decimal amount)
        {
            Instalments.Add(new Installment(Guid.NewGuid().ToString(), dueDate, amount));
        }
        public void PayInstallment(string installmentId, DateTime paymentDate)
        {
            Instalments.Single(i => i.Id == installmentId).PaidDate = paymentDate;
        }
        public void ReversePayment(string installmentId, DateTime reversalDate)
        {
            Instalments.Single(i => i.Id == installmentId).PaidDate = reversalDate;
        }
    }
}
