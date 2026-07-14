using PasswordGeneratorApi.Services;

namespace PasswordGeneratorApi.Tests;

public sealed class PasswordStrengthCalculatorTests
{
    [Theory]
    [InlineData(6, 10, "Weak")]
    [InlineData(12, 62, "Strong")]
    [InlineData(20, 80, "Very strong")]
    public void Calculate_ReturnsExpectedStrength(
        int length,
        int poolSize,
        string expected)
    {
        var result = PasswordStrengthCalculator.Calculate(length, poolSize);

        Assert.Equal(expected, result);
    }
}
