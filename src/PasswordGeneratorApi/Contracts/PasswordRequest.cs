namespace PasswordGeneratorApi.Contracts;

public sealed record PasswordRequest
{
    public int Length { get; init; } = 16;

    public bool IncludeUppercase { get; init; } = true;

    public bool IncludeLowercase { get; init; } = true;

    public bool IncludeNumbers { get; init; } = true;

    public bool IncludeSpecialCharacters { get; init; } = true;

    public string? ExcludedCharacters { get; init; }
}
