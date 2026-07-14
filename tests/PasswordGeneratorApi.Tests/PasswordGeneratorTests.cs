using PasswordGeneratorApi.Services;

namespace PasswordGeneratorApi.Tests;

public sealed class PasswordGeneratorTests
{
    private readonly PasswordGenerator _generator = new();

    [Fact]
    public void Generate_ReturnsPasswordWithRequestedLength()
    {
        var options = new PasswordOptions
        {
            Length = 24
        };

        var result = _generator.Generate(options);

        Assert.Equal(24, result.Password.Length);
        Assert.Equal(24, result.Length);
    }

    [Fact]
    public void Generate_DoesNotUseExcludedCharacters()
    {
        var options = new PasswordOptions
        {
            Length = 64,
            ExcludedCharacters = "0O1l"
        };

        var result = _generator.Generate(options);

        Assert.DoesNotContain('0', result.Password);
        Assert.DoesNotContain('O', result.Password);
        Assert.DoesNotContain('1', result.Password);
        Assert.DoesNotContain('l', result.Password);
    }

    [Fact]
    public void Generate_UsesOnlyEnabledCharacterGroups()
    {
        var options = new PasswordOptions
        {
            Length = 32,
            IncludeUppercase = false,
            IncludeLowercase = false,
            IncludeNumbers = true,
            IncludeSpecialCharacters = false
        };

        var result = _generator.Generate(options);

        Assert.All(result.Password, character => Assert.True(char.IsDigit(character)));
    }

    [Fact]
    public void Generate_ThrowsWhenNoCharacterGroupIsEnabled()
    {
        var options = new PasswordOptions
        {
            IncludeUppercase = false,
            IncludeLowercase = false,
            IncludeNumbers = false,
            IncludeSpecialCharacters = false
        };

        Assert.Throws<InvalidOperationException>(
            () => _generator.Generate(options));
    }
}
