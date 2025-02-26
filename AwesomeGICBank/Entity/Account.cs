using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApplication.Entity
{
    public class Account
    {
        public string AccountNumber { get; private set; }
        public decimal Balance { get; private set; }
        public List<Transaction> Transactions { get; private set; }

        public Account(string accountNumber)
        {
            AccountNumber = accountNumber;
            Balance = 0;
            Transactions = new List<Transaction>();
        }

        public bool AddTransaction(Transaction transaction)
        {
            Transactions.Add(transaction);
            if (transaction.Type == 'D')
                Balance += transaction.Amount;
            else if (transaction.Type == 'W')
                Balance -= transaction.Amount;

            return true; // Transaction successful
        }
    }
}
