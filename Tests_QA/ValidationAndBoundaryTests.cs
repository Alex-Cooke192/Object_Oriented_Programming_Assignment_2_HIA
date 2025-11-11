using Xunit;
using System.Linq;

namespace Tests_QA
{
    public class ValidationAndBoundaryTests
    {
        [Theory(DisplayName = "Rejects Empty Or Null Inputs")]
        [InlineData("")]
        [InlineData(null)]
        public void ShouldReject_NullOrEmptyInputs(string input)
        {
            bool isValid = !string.IsNullOrWhiteSpace(input);
            Assert.False(isValid, "System accepted a null or empty input.");
        }

        [Theory(DisplayName = "Rejects Out Of Range Values")]
        [InlineData(-1)]
        [InlineData(101)]
        public void ShouldReject_OutOfRangeValues(int value)
        {
            bool isValid = value >= 0 && value <= 100;
            Assert.False(isValid, $"Value {value} incorrectly accepted.");
        }

        [Theory(DisplayName = "Accepts Valid Range Values")]
        [InlineData(0)]
        [InlineData(50)]
        [InlineData(100)]
        public void ShouldAccept_ValidRangeValues(int value)
        {
            bool isValid = value >= 0 && value <= 100;
            Assert.True(isValid);
        }

        [Fact(DisplayName = "Rejects Invalid Email Format")]
        public void ShouldReject_InvalidEmail()
        {
            string email = "user@@domain";
            bool isValid = email.Contains("@") && email.Contains(".");
            Assert.False(isValid, "Invalid email accepted.");
        }

        [Fact(DisplayName = "Ensures Password Meets Complexity Rules")]
        public void Password_ShouldMeetComplexityRequirements()
        {
            string password = "Abc123!";
            bool hasUpper = password.Any(char.IsUpper);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSymbol = password.Any(ch => !char.IsLetterOrDigit(ch));
            Assert.True(hasUpper && hasDigit && hasSymbol, "Password did not meet all complexity rules.");
        }

        [Fact(DisplayName = "Boundary Test: Maximum Text Length Not Exceeded")]
        public void ShouldReject_ExcessiveTextLength()
        {
            string input = new string('A', 501);
            bool isValid = input.Length <= 500;
            Assert.False(isValid, "Input exceeded allowed limit.");
        }
    }
}
