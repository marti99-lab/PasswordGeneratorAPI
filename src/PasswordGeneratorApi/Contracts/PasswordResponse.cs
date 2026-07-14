namespace PasswordGeneratorApi.Contracts;

public sealed record PasswordResponse(
    string Password,
    int Length,
    string Strength,
    int CharacterPoolSize);
