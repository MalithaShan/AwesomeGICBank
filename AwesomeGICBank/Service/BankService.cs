using BankingApplication.Entity;
using BankingApplication.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApplication.Service
{
    public class BankService : IBankService
    {
        private Dictionary<string, Account> accounts = new Dictionary<string, Account>();
        private List<InterestRule> interestRules = new List<InterestRule>();
        private readonly IValidationService _validationService;

        public BankService(IValidationService validationService)
        {
            _validationService = validationService;
        }

        #region Transaction

        public bool AddTransaction(string accountNumber, DateTime date, char type, decimal amount)
        {
            if (!accounts.ContainsKey(accountNumber))
            {
                accounts[accountNumber] = new Account(accountNumber);
            }

            var account = accounts[accountNumber];

            // Validate the transaction
            if (!_validationService.ValidateTransaction(account, type, amount))
            {
                return false; // Validation failed
            }

            var transactionId = $"{date:yyyyMMdd}-{account.Transactions.Count + 1:D2}";
            var transaction = new Transaction(transactionId, date, type, amount);

            return account.AddTransaction(transaction);
        }
        public List<Transaction> GetTransactionsForAccount(string accountNumber)
        {
            if (accounts.ContainsKey(accountNumber))
            {
                return accounts[accountNumber].Transactions;
            }
            return new List<Transaction>();
        }

        #endregion

        #region Interest Rules
        public bool AddInterestRule(DateTime date, string ruleId, decimal rate)
        {
            // Validate the interest rate
            if (!_validationService.ValidateInterestRate(rate))
            {
                return false; // Validation failed
            }

            var existingRule = interestRules.FirstOrDefault(r => r.Date == date);
            if (existingRule != null)
            {
                interestRules.Remove(existingRule);
            }

            interestRules.Add(new InterestRule(date, ruleId, rate));
            return true; // Interest rule added successfully
        }
        public List<InterestRule> GetInterestRules()
        {
            return interestRules.OrderBy(r => r.Date).ToList();
        }
        #endregion

        #region Print
        public string GenerateStatement(string accountNumber, int year, int month)
        
        
        {
            if (!accounts.ContainsKey(accountNumber))
            {
                return "Account not found.";
            }

            var account = accounts[accountNumber];
            var transactions = account.Transactions
                .Where(t => t.Date.Year == year && t.Date.Month == month)
                .OrderBy(t => t.Date)
                .ToList();

            var balance = account.Transactions
                .Where(t => t.Date.Year <= year && t.Date.Month < month)
                .ToList().Sum(x=>x.Amount);

            // Calculate daily balances
            var dailyBalances = CalculateDailyBalances(account, year, month, balance);

            // Calculate interest
            var interest = CalculateInterest(dailyBalances, year, month);

            // Add interest transaction
            if (interest > 0)
            {
                var lastDayOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                var interestTransaction = new Transaction(string.Empty.PadRight(11), lastDayOfMonth, 'I', interest);
                transactions.Add(interestTransaction);
            }

            // Generate statement
            var statement = $"Account: {accountNumber}\n";
            statement += "| Date     | Txn Id      | Type | Amount  | Balance |\n";

            
            foreach (var transaction in transactions.OrderBy(t => t.Date))
            {
                balance += transaction.Type == 'W' ? -transaction.Amount : transaction.Amount;
                statement += $"| {transaction.Date:yyyyMMdd} | {transaction.TransactionId} | {transaction.Type.ToString().PadRight(4)} | {transaction.Amount,7:F2} | {balance,7:F2} |\n";
            }

            return statement;
        }
        private Dictionary<DateTime, decimal> CalculateDailyBalances(Account account, int year, int month,decimal balance)
        {
            var dailyBalances = new Dictionary<DateTime, decimal>();

            var startDate = new DateTime(year, month, 1);
            var endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var transactionsForDay = account.Transactions
                    .Where(t => t.Date.Date == date.Date)
                    .OrderBy(t => t.Date)
                    .ToList();

                foreach (var transaction in transactionsForDay)
                {
                    balance += transaction.Type == 'D' ? transaction.Amount : -transaction.Amount;
                }

                dailyBalances[date] = balance;
            }

            return dailyBalances;
        }
        private decimal CalculateInterest(Dictionary<DateTime, decimal> dailyBalances, int year, int month)
        {
            decimal totalInterest = 0;

            var startDate = new DateTime(year, month, 1);
            var endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            var rules = interestRules
                .Where(r => r.Date <= endDate)
                .OrderBy(r => r.Date)
                .ToList();

            if (rules.Count == 0)
            {
                return 0; // No interest rules defined
            }

            var currentRule = rules[0];
            var nextRuleIndex = 1;

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                // Check if a new rule applies
                if (nextRuleIndex < rules.Count && date >= rules[nextRuleIndex].Date)
                {
                    currentRule = rules[nextRuleIndex];
                    nextRuleIndex++;
                }

                // Calculate daily interest
                var dailyInterest = dailyBalances[date] * currentRule.Rate / 100 / 365;
                totalInterest += dailyInterest;
            }

            return Math.Round(totalInterest, 2); // Round to 2 decimal places
        }
        #endregion
    }
}
