using System.Security.Cryptography;
using PasswordGeneratorApi.Contracts;

namespace PasswordGeneratorApi.Services;

public sealed class PasswordGenerator : IPasswordGenerator
{
    private const string UppercaseCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string LowercaseCharacters = "abcdefghijklmnopqrstuvwxyz";
    private const string NumberCharacters = "0123456789";
    private const string SpecialCharacters = "!@#$%^&*()-_=+[]{};:,.?";

    public PasswordResponse Generate(PasswordOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var selectedGroups = BuildCharacterGroups(options);

        if (selectedGroups.Count == 0)
        {
            throw new InvalidOperationException(
                "At least one character group must be enabled.");
        }

        var characterPool = string.Concat(selectedGroups);

        if (string.IsNullOrEmpty(characterPool))
        {
            throw new InvalidOperationException(
                "The excluded characters remove the entire character pool.");
        }

        var characters = new List<char>(options.Length);

        // Add one character from each enabled group where possible.
        // This prevents a generated password from accidentally missing
        // a character type that the caller explicitly requested.
        foreach (var group in selectedGroups.Take(options.Length))
        {
            characters.Add(GetRandomCharacter(group));
        }

        while (characters.Count < options.Length)
        {
            characters.Add(GetRandomCharacter(characterPool));
        }

        Shuffle(characters);

        var password = new string(characters.ToArray());
        var strength = PasswordStrengthCalculator.Calculate(
            options.Length,
            characterPool.Length);

        return new PasswordResponse(
            password,
            password.Length,
            strength,
            characterPool.Length);
    }

    private static List<string> BuildCharacterGroups(PasswordOptions options)
    {
        var excluded = options.ExcludedCharacters.ToHashSet();
        var groups = new List<string>();

        AddGroup(groups, UppercaseCharacters, options.IncludeUppercase, excluded);
        AddGroup(groups, LowercaseCharacters, options.IncludeLowercase, excluded);
        AddGroup(groups, NumberCharacters, options.IncludeNumbers, excluded);
        AddGroup(
            groups,
            SpecialCharacters,
            options.IncludeSpecialCharacters,
            excluded);

        return groups;
    }

    private static void AddGroup(
        ICollection<string> groups,
        string source,
        bool isEnabled,
        HashSet<char> excluded)
    {
        if (!isEnabled)
        {
            return;
        }

        var availableCharacters = new string(
            source.Where(character => !excluded.Contains(character)).ToArray());

        if (!string.IsNullOrEmpty(availableCharacters))
        {
            groups.Add(availableCharacters);
        }
    }

    private static char GetRandomCharacter(string source)
    {
        var index = RandomNumberGenerator.GetInt32(source.Length);
        return source[index];
    }

    private static void Shuffle(IList<char> characters)
    {
        for (var currentIndex = characters.Count - 1;
             currentIndex > 0;
             currentIndex--)
        {
            var randomIndex = RandomNumberGenerator.GetInt32(currentIndex + 1);

            (characters[currentIndex], characters[randomIndex]) =
                (characters[randomIndex], characters[currentIndex]);
        }
    }
}
