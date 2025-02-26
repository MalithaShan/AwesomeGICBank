using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using BankingApplication.Entity;
using BankingApplication.Interface;

namespace BankingApplication.Service
{
    public class ValidationService : IValidationService
    {
        #region Transaction
        public bool ValidateTransaction(Account account, char type, decimal amount)
        {
            if (type == 'W' && account.Balance - amount < 0)
            {
                return false; // Insufficient balance for withdrawal
            }
            return true; // Transaction is valid
        }
        public bool ValidateInputTransaction(string[] input)
        {
            // Check if input is null or doesn't have exactly 4 elements
            if (input == null || input.Length != 4)
            {
                return false;
            }

            // Check if input[1] is null or empty
            if (string.IsNullOrEmpty(input[1]))
            {
                return false;
            }

            // Check if the first character of input[2] is 'w' or 'd' (case-insensitive)
            if (input[2].Length == 0 || (char.ToLower(input[2][0]) != 'w' && char.ToLower(input[2][0]) != 'd'))
            {
                return false;
            }

            // Validate date and value in a single return statement
            return DateTime.TryParseExact(input[0], "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out _) &&
                   decimal.TryParse(input[3], out _);
        }
        #endregion

        #region Interest Rules
        public bool ValidateInterestRate(decimal rate)
        {
            return rate > 0 && rate < 100; // Rate must be between 0 and 100
        }
        public bool ValidationInterestRule(string[] input)
        {
            if (input == null || input.Length != 3)
            {
                return false;
            }

            // Check if input[1] is null or empty
            if (string.IsNullOrEmpty(input[1]))
            {
                return false;
            }

            // Validate date and value in a single return statement
            return DateTime.TryParseExact(input[0], "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out _) &&
                   decimal.TryParse(input[2], out _);
        }
        #endregion

    }
}
