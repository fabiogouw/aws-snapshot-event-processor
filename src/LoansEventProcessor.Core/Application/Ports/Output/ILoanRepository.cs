using LoansEventProcessor.Core.Application.Ports.Input;
using LoansEventProcessor.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LoansEventProcessor.Core.Application.Ports.Output
{
    public interface ILoanRepository
    {
        Task<Snapshot<Loan>> GetLoan(string loanId);
        Task<bool> SaveLoan(Snapshot<Loan> loan);
    }
}
