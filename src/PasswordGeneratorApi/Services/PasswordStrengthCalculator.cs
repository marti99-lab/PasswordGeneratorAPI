namespace PasswordGeneratorApi.Services;

public static class PasswordStrengthCalculator
{
    public static string Calculate(int length, int characterPoolSize)
    {
        if (length <= 0 || characterPoolSize <= 0)
        {
            return "Very weak";
        }

        var entropy = length * Math.Log2(characterPoolSize);

        return entropy switch
        {
            < 36 => "Weak",
            < 60 => "Medium",
            < 80 => "Strong",
            _ => "Very strong"
        };
    }
}
