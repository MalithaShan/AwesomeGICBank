using BankingApplication.Interface;
using BankingApplication.Service;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;

namespace BankingApplication
{
    public class BankApplication
    {
        private static IBankService _bankService;
        private static IValidationService _validationService;
        
        public static void Main(string[] args)
        {
            SetupService();
            Applaunch();
        }

        #region Setup Method
        public static void Applaunch()
        {
            bool running = true;
            while (running)
            {
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine(ConfigurationManager.AppSettings["WelcomeMessage"]);
                Console.WriteLine(ConfigurationManager.AppSettings["InputTransactionOption"]);
                Console.WriteLine(ConfigurationManager.AppSettings["DefineInterestRuleOption"]);
                Console.WriteLine(ConfigurationManager.AppSettings["PrintStatementOption"]);
                Console.WriteLine(ConfigurationManager.AppSettings["QuitOption"]);
                Console.Write(ConfigurationManager.AppSettings["EnterSymbol"]);
                var input = Console.ReadLine().ToUpper();

                switch (input)
                {
                    case "T":
                        InputTransaction();
                        break;
                    case "I":
                        DefineInterestRule();
                        break;
                    case "P":
                        PrintStatement();
                        break;
                    case "Q":
                        running = false;
                        Console.WriteLine(ConfigurationManager.AppSettings["GoodbyeMessage"]);
                        Console.ReadKey();
                        break;
                    default:
                        Console.WriteLine(ConfigurationManager.AppSettings["InvalidOptionMessage"]);
                        break;
                }
            }
            
        }
        public static void SetupService()
        {
            // Set up dependency injection
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IValidationService, ValidationService>()
                .AddSingleton<IBankService, BankService>()
                .BuildServiceProvider();

            _bankService = serviceProvider.GetService<IBankService>();
            _validationService = serviceProvider.GetService<IValidationService>();
        }

        #endregion

        #region Function Method
        public static void InputTransaction()
        {
            try
            {
                Console.WriteLine(ConfigurationManager.AppSettings["EnterTransactionDetails"]);
                Console.Write(ConfigurationManager.AppSettings["EnterSymbol"]);
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) return;

                Console.WriteLine(Environment.NewLine);
                var parts = input.Split(' ');
                if (_validationService.ValidateInputTransaction(parts))
                {
                    DateTime.TryParseExact(parts[0], "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime date);
                    bool success = _bankService.AddTransaction(parts[1], date, parts[2][0], decimal.Parse(parts[3]));
                    if (success)
                    {
                        Console.WriteLine(ConfigurationManager.AppSettings["TransactionSuccess"]);
                    }
                    else
                    {
                        Console.WriteLine(ConfigurationManager.AppSettings["TransactionFailed"]);
                    }
                }
                else
                {
                    Console.WriteLine(ConfigurationManager.AppSettings["InvalidTransactionFormat"]);
                }

            }
            catch 
            {
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine(ConfigurationManager.AppSettings["TransactionFailed"]);

            }
            
        }
        public static void DefineInterestRule()
        {
            try
            {
                Console.WriteLine(ConfigurationManager.AppSettings["EnterInterestRuleDetails"]);
                Console.Write(ConfigurationManager.AppSettings["EnterSymbol"]);
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) return;

                Console.WriteLine(Environment.NewLine);
                var parts = input.Split(' ');
                if (_validationService.ValidationInterestRule(parts))
                {

                    DateTime.TryParseExact(parts[0], "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime date);
                    bool success = _bankService.AddInterestRule(date, parts[1], decimal.Parse(parts[2]));
                    if (success)
                    {
                        Console.WriteLine(ConfigurationManager.AppSettings["InterestRuleSuccess"]);
                    }
                    else
                    {
                        Console.WriteLine(ConfigurationManager.AppSettings["InterestRuleFailed"]);
                    }
                }
                else
                {
                    Console.WriteLine(ConfigurationManager.AppSettings["InvalidInterestRuleFormat"]);
                }

            }
            catch
            {
                Console.WriteLine(ConfigurationManager.AppSettings["InterestRuleFailed"]);

            }

        }
        public static void PrintStatement()
        {
            Console.WriteLine(ConfigurationManager.AppSettings["EnterStatementDetails"]);
            Console.Write(ConfigurationManager.AppSettings["EnterSymbol"]);
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) return;

            Console.WriteLine(Environment.NewLine);
            var parts = input.Split(' ');
            if (parts.Length == 2 && int.TryParse(parts[1].Substring(0, 4), out int year) && int.TryParse(parts[1].Substring(4), out int month))
            {
                var statement = _bankService.GenerateStatement(parts[0], year, month);
                Console.WriteLine(statement);
            }
            else
            {
                Console.WriteLine(ConfigurationManager.AppSettings["InvalidStatementFormat"]);
            }
        }

        #endregion
    }
}
