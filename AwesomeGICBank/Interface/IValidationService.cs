using BankingApplication.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApplication.Interface
{
    public interface IValidationService
    {
        bool ValidateTransaction(Account account, char type, decimal amount);
        bool ValidateInterestRate(decimal rate);
        bool ValidateInputTransaction(string[] input);
        bool ValidationInterestRule(string[] input);
    }
}
