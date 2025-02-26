using BankingApplication.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using BankingApplication.Interface;
using Moq;
using BankingApplication.Entity;
using System.Collections.Generic;

namespace AwesomeGIC.Tests
{
    [TestClass]
    public class BankServiceTests
    {
        private Mock<IValidationService> _mockValidationService;
        private IBankService _bankService;

        [TestInitialize]
        public void Setup()
        {
            _mockValidationService = new Mock<IValidationService>();
            _bankService = new BankService(_mockValidationService.Object);
        }

        // Test Case 1: AddTransaction_NewAccount_ShouldCreateAccountAndAddTransaction
        [TestMethod]
        public void AddTransaction_NewAccount_ShouldCreateAccountAndAddTransaction()
        {
            // Arrange
            _mockValidationService.Setup(v => v.ValidateTransaction(It.IsAny<Account>(), 'D', 100.00m)).Returns(true);

            // Act
            var result = _bankService.AddTransaction("AC001", new DateTime(2023, 6, 26), 'D', 100.00m);

            // Assert
            Assert.IsTrue(result);
            var transactions = _bankService.GetTransactionsForAccount("AC001");
            Assert.AreEqual(1, transactions.Count);
            Assert.AreEqual("20230626-01", transactions[0].TransactionId);
            Assert.AreEqual(100.00m, transactions[0].Amount);
        }

        // Test Case 2: AddTransaction_WithdrawalWithInsufficientBalance_ShouldReturnFalse
        [TestMethod]
        public void AddTransaction_WithdrawalWithInsufficientBalance_ShouldReturnFalse()
        {
            // Arrange
            _mockValidationService.Setup(v => v.ValidateTransaction(It.IsAny<Account>(), 'W', 100.00m)).Returns(false);

            // Act
            var result = _bankService.AddTransaction("AC001", new DateTime(2023, 6, 26), 'W', 100.00m);

            // Assert
            Assert.IsFalse(result);
        }

        // Test Case 3: AddInterestRule_ValidRule_ShouldReturnTrue
        [TestMethod]
        public void AddInterestRule_ValidRule_ShouldReturnTrue()
        {
            // Arrange
            _mockValidationService.Setup(v => v.ValidateInterestRate(2.20m)).Returns(true);

            // Act
            var result = _bankService.AddInterestRule(new DateTime(2023, 6, 15), "RULE03", 2.20m);

            // Assert
            Assert.IsTrue(result);
            var rules = _bankService.GetInterestRules();
            Assert.AreEqual(1, rules.Count);
            Assert.AreEqual("RULE03", rules[0].RuleId);
            Assert.AreEqual(2.20m, rules[0].Rate);
        }

        // Test Case 4: AddInterestRule_InvalidRate_ShouldReturnFalse
        [TestMethod]
        public void AddInterestRule_InvalidRate_ShouldReturnFalse()
        {
            // Arrange
            _mockValidationService.Setup(v => v.ValidateInterestRate(101.00m)).Returns(false);

            // Act
            var result = _bankService.AddInterestRule(new DateTime(2023, 6, 15), "RULE03", 101.00m);

            // Assert
            Assert.IsFalse(result);
        }

        // Test Case 5: GetTransactionsForAccount_NoTransactions_ShouldReturnEmptyList
        [TestMethod]
        public void GetTransactionsForAccount_NoTransactions_ShouldReturnEmptyList()
        {
            // Act
            var transactions = _bankService.GetTransactionsForAccount("AC001");

            // Assert
            Assert.AreEqual(0, transactions.Count);
        }

        // Test Case 6: GetInterestRules_MultipleRules_ShouldReturnOrderedList
        [TestMethod]
        public void GetInterestRules_MultipleRules_ShouldReturnOrderedList()
        {
            // Arrange
            _mockValidationService.Setup(v => v.ValidateInterestRate(2.20m)).Returns(true);
            _mockValidationService.Setup(v => v.ValidateInterestRate(1.95m)).Returns(true);

            _bankService.AddInterestRule(new DateTime(2023, 6, 15), "RULE03", 2.20m);
            _bankService.AddInterestRule(new DateTime(2023, 1, 1), "RULE01", 1.95m);

            // Act
            var rules = _bankService.GetInterestRules();

            // Assert
            Assert.AreEqual(2, rules.Count);
            Assert.AreEqual("RULE01", rules[0].RuleId);
            Assert.AreEqual("RULE03", rules[1].RuleId);
        }

        // Test Case 7: AddTransaction_FirstTransactionIsWithdrawal_ShouldReturnFalse
        [TestMethod]
        public void AddTransaction_FirstTransactionIsWithdrawal_ShouldReturnFalse()
        {
            // Arrange
            _mockValidationService.Setup(v => v.ValidateTransaction(It.IsAny<Account>(), 'W', 100.00m)).Returns(false);

            // Act
            var result = _bankService.AddTransaction("AC001", new DateTime(2023, 6, 26), 'W', 100.00m);

            // Assert
            Assert.IsFalse(result);
        }

        // Test Case 8: AddTransaction_Deposit_ShouldIncreaseBalance
        [TestMethod]
        public void AddTransaction_Deposit_ShouldIncreaseBalance()
        {
            // Arrange
            _mockValidationService.Setup(v => v.ValidateTransaction(It.IsAny<Account>(), 'D', 200.00m)).Returns(true);

            // Act
            var result = _bankService.AddTransaction("AC001", new DateTime(2023, 6, 26), 'D', 200.00m);

            // Assert
            Assert.IsTrue(result);
            var transactions = _bankService.GetTransactionsForAccount("AC001");
            Assert.AreEqual(1, transactions.Count);
            Assert.AreEqual(200.00m, transactions[0].Amount);
            Assert.AreEqual('D', transactions[0].Type);
        }

        // Test Case 9: AddTransaction_Withdrawal_ShouldDecreaseBalance
        [TestMethod]
        public void AddTransaction_Withdrawal_ShouldDecreaseBalance()
        {
            // Arrange
            _mockValidationService.Setup(v => v.ValidateTransaction(It.IsAny<Account>(), 'D', 200.00m)).Returns(true);
            _mockValidationService.Setup(v => v.ValidateTransaction(It.IsAny<Account>(), 'W', 100.00m)).Returns(true);

            // First, deposit some money
            _bankService.AddTransaction("AC001", new DateTime(2023, 6, 26), 'D', 200.00m);

            // Then, withdraw some money
            var result = _bankService.AddTransaction("AC001", new DateTime(2023, 6, 27), 'W', 100.00m);

            // Assert
            Assert.IsTrue(result);
            var transactions = _bankService.GetTransactionsForAccount("AC001");
            Assert.AreEqual(2, transactions.Count);
            Assert.AreEqual(100.00m, transactions[1].Amount);
            Assert.AreEqual('W', transactions[1].Type);
        }

        // Test Case 10: AddTransaction_InvalidTransactionType_ShouldReturnFalse
        [TestMethod]
        public void AddTransaction_InvalidTransactionType_ShouldReturnFalse()
        {
            // Arrange
            _mockValidationService.Setup(v => v.ValidateTransaction(It.IsAny<Account>(), 'X', 100.00m)).Returns(false);

            // Act
            var result = _bankService.AddTransaction("AC001", new DateTime(2023, 6, 26), 'X', 100.00m);

            // Assert
            Assert.IsFalse(result);
        }

        // Test Case 11: AddInterestRule_ExistingRule_ShouldReplaceRule
        [TestMethod]
        public void AddInterestRule_ExistingRule_ShouldReplaceRule()
        {
            // Arrange
            _mockValidationService.Setup(v => v.ValidateInterestRate(2.20m)).Returns(true);
            _mockValidationService.Setup(v => v.ValidateInterestRate(3.00m)).Returns(true);

            // Add an initial rule
            _bankService.AddInterestRule(new DateTime(2023, 6, 15), "RULE03", 2.20m);

            // Add a new rule with the same date and rule ID
            var result = _bankService.AddInterestRule(new DateTime(2023, 6, 15), "RULE03", 3.00m);

            // Assert
            Assert.IsTrue(result);
            var rules = _bankService.GetInterestRules();
            Assert.AreEqual(1, rules.Count);
            Assert.AreEqual(3.00m, rules[0].Rate);
        }

        // Test Case 12: GetTransactionsForAccount_AccountDoesNotExist_ShouldReturnEmptyList
        [TestMethod]
        public void GetTransactionsForAccount_AccountDoesNotExist_ShouldReturnEmptyList()
        {
            // Act
            var transactions = _bankService.GetTransactionsForAccount("NON_EXISTENT_ACCOUNT");

            // Assert
            Assert.AreEqual(0, transactions.Count);
        }

        // Test Case 13: GetInterestRules_NoRules_ShouldReturnEmptyList
        [TestMethod]
        public void GetInterestRules_NoRules_ShouldReturnEmptyList()
        {
            // Act
            var rules = _bankService.GetInterestRules();

            // Assert
            Assert.AreEqual(0, rules.Count);
        }

        // Test Case 14: GenerateStatement_WithInterest_ShouldIncludeInterestTransaction
        [TestMethod]
        public void GenerateStatement_WithInterest_ShouldIncludeInterestTransaction()
        {
            // Arrange
            _mockValidationService.Setup(v => v.ValidateTransaction(It.IsAny<Account>(), 'D', 100.00m)).Returns(true);
            _mockValidationService.Setup(v => v.ValidateTransaction(It.IsAny<Account>(), 'D', 150.00m)).Returns(true);
            _mockValidationService.Setup(v => v.ValidateTransaction(It.IsAny<Account>(), 'W', 20.00m)).Returns(true);
            _mockValidationService.Setup(v => v.ValidateTransaction(It.IsAny<Account>(), 'W', 100.00m)).Returns(true);
            _mockValidationService.Setup(v => v.ValidateInterestRate(1.90m)).Returns(true);
            _mockValidationService.Setup(v => v.ValidateInterestRate(2.20m)).Returns(true);


            _bankService.AddTransaction("AC001", new DateTime(2023, 5, 5), 'D', 100.00m);
            _bankService.AddTransaction("AC001", new DateTime(2023, 6, 1), 'D', 150.00m);
            _bankService.AddTransaction("AC001", new DateTime(2023, 6, 26), 'W', 20.00m);
            _bankService.AddTransaction("AC001", new DateTime(2023, 6, 26), 'W', 100.00m);

            _bankService.AddInterestRule(new DateTime(2023, 1, 1), "RULE01", 1.95m);
            _bankService.AddInterestRule(new DateTime(2023, 5, 20), "RULE02", 1.90m);
            _bankService.AddInterestRule(new DateTime(2023, 6, 15), "RULE03", 2.20m);

            // Act
            var statement = _bankService.GenerateStatement("AC001", 2023, 6);

            // Assert
            Assert.IsTrue(statement.Contains("| 20230630 |             | I    |    0.39 |  130.39 |"));
        }

        [TestMethod]
        public void GenerateStatement_AccountDoesNotExist_ShouldReturnErrorMessage()
        {
            // Act
            var statement = _bankService.GenerateStatement("NON_EXISTENT_ACCOUNT", 2023, 6);

            // Assert
            Assert.AreEqual("Account not found.", statement);
        }

        [TestMethod]
        public void GenerateStatement_NoTransactions_ShouldReturnEmptyStatement()
        {
            // Arrange
            _bankService.AddTransaction("AC001", new DateTime(2023, 6, 1), 'D', 100.00m);

            // Act
            var statement = _bankService.GenerateStatement("AC001", 2023, 7); // No transactions in July

            // Assert
            Assert.IsTrue(statement.Contains("| Date     | Txn Id      | Type | Amount  | Balance |"));
            Assert.IsFalse(statement.Contains("| 20230601 | 20230601-01 | D    | 100.00 |  100.00 |"));
        }

        [TestMethod]
        public void GenerateStatement_NoInterestRules_ShouldNotIncludeInterest()
        {
            // Arrange
            _bankService.AddTransaction("AC001", new DateTime(2023, 6, 1), 'D', 100.00m);

            // Act
            var statement = _bankService.GenerateStatement("AC001", 2023, 6);

            // Assert
            Assert.IsFalse(statement.Contains("| 20230630 |             | I    |"));
        }
    }
}
