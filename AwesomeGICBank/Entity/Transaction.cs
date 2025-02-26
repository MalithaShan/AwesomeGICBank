using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApplication.Entity
{
    public class Transaction
    {
        public string TransactionId { get; private set; }
        public DateTime Date { get; private set; }
        public char Type { get; private set; }
        public decimal Amount { get; private set; }

        public Transaction(string transactionId, DateTime date, char type, decimal amount)
        {
            TransactionId = transactionId;
            Date = date;
            Type = type;
            Amount = amount;
        }
    }
}
