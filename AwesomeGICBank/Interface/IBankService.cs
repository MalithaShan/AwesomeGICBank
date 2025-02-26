using BankingApplication.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApplication.Interface
{
    public interface IBankService
    {
        bool AddTransaction(string accountNumber, DateTime date, char type, decimal amount);
        bool AddInterestRule(DateTime date, string ruleId, decimal rate);
        List<Transaction> GetTransactionsForAccount(string accountNumber);
        List<InterestRule> GetInterestRules();
        string GenerateStatement(string accountNumber, int year, int month);
    }
}
