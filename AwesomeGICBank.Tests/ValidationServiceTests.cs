using BankingApplication.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AwesomeGIC.Tests
{
    [TestClass]
    public class ValidationServiceTests
    {
        private Mock<IValidationService> _mockValidationService;

        [TestInitialize]
        public void Setup()
        {
            _mockValidationService = new Mock<IValidationService>();
        }

        // Test cases for ValidateInputTransaction
        [TestMethod]
        public void ValidateInputTransaction_ValidInput_ShouldReturnTrue()
        {
            // Arrange
            var input = new[] { "20230626", "AC001", "D", "100.00" };
            _mockValidationService.Setup(v => v.ValidateInputTransaction(input)).Returns(true);

            // Act
            var result = _mockValidationService.Object.ValidateInputTransaction(input);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateInputTransaction_NullInput_ShouldReturnFalse()
        {
            // Arrange
            string[] input = null;
            _mockValidationService.Setup(v => v.ValidateInputTransaction(input)).Returns(false);

            // Act
            var result = _mockValidationService.Object.ValidateInputTransaction(input);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateInputTransaction_IncorrectLength_ShouldReturnFalse()
        {
            // Arrange
            var input = new[] { "20230626", "AC001", "D" }; // Missing amount
            _mockValidationService.Setup(v => v.ValidateInputTransaction(input)).Returns(false);

            // Act
            var result = _mockValidationService.Object.ValidateInputTransaction(input);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateInputTransaction_EmptyAccount_ShouldReturnFalse()
        {
            // Arrange
            var input = new[] { "20230626", "", "D", "100.00" }; // Empty account
            _mockValidationService.Setup(v => v.ValidateInputTransaction(input)).Returns(false);

            // Act
            var result = _mockValidationService.Object.ValidateInputTransaction(input);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateInputTransaction_InvalidTransactionType_ShouldReturnFalse()
        {
            // Arrange
            var input = new[] { "20230626", "AC001", "X", "100.00" }; // Invalid type
            _mockValidationService.Setup(v => v.ValidateInputTransaction(input)).Returns(false);

            // Act
            var result = _mockValidationService.Object.ValidateInputTransaction(input);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateInputTransaction_InvalidDate_ShouldReturnFalse()
        {
            // Arrange
            var input = new[] { "20231326", "AC001", "D", "100.00" }; // Invalid date
            _mockValidationService.Setup(v => v.ValidateInputTransaction(input)).Returns(false);

            // Act
            var result = _mockValidationService.Object.ValidateInputTransaction(input);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateInputTransaction_InvalidAmount_ShouldReturnFalse()
        {
            // Arrange
            var input = new[] { "20230626", "AC001", "D", "ABC" }; // Invalid amount
            _mockValidationService.Setup(v => v.ValidateInputTransaction(input)).Returns(false);

            // Act
            var result = _mockValidationService.Object.ValidateInputTransaction(input);

            // Assert
            Assert.IsFalse(result);
        }

        // Test cases for ValidationInterestRule
        [TestMethod]
        public void ValidationInterestRule_ValidInput_ShouldReturnTrue()
        {
            // Arrange
            var input = new[] { "20230626", "RULE01", "2.20" };
            _mockValidationService.Setup(v => v.ValidationInterestRule(input)).Returns(true);

            // Act
            var result = _mockValidationService.Object.ValidationInterestRule(input);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidationInterestRule_NullInput_ShouldReturnFalse()
        {
            // Arrange
            string[] input = null;
            _mockValidationService.Setup(v => v.ValidationInterestRule(input)).Returns(false);

            // Act
            var result = _mockValidationService.Object.ValidationInterestRule(input);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidationInterestRule_IncorrectLength_ShouldReturnFalse()
        {
            // Arrange
            var input = new[] { "20230626", "RULE01" }; // Missing rate
            _mockValidationService.Setup(v => v.ValidationInterestRule(input)).Returns(false);

            // Act
            var result = _mockValidationService.Object.ValidationInterestRule(input);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidationInterestRule_EmptyRuleId_ShouldReturnFalse()
        {
            // Arrange
            var input = new[] { "20230626", "", "2.20" }; // Empty rule ID
            _mockValidationService.Setup(v => v.ValidationInterestRule(input)).Returns(false);

            // Act
            var result = _mockValidationService.Object.ValidationInterestRule(input);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidationInterestRule_InvalidDate_ShouldReturnFalse()
        {
            // Arrange
            var input = new[] { "20231326", "RULE01", "2.20" }; // Invalid date
            _mockValidationService.Setup(v => v.ValidationInterestRule(input)).Returns(false);

            // Act
            var result = _mockValidationService.Object.ValidationInterestRule(input);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidationInterestRule_InvalidRate_ShouldReturnFalse()
        {
            // Arrange
            var input = new[] { "20230626", "RULE01", "ABC" }; // Invalid rate
            _mockValidationService.Setup(v => v.ValidationInterestRule(input)).Returns(false);

            // Act
            var result = _mockValidationService.Object.ValidationInterestRule(input);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
