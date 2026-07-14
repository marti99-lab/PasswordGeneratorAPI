using PasswordGeneratorApi.Contracts;

namespace PasswordGeneratorApi.Validation;

public static class PasswordRequestValidator
{
    public static Dictionary<string, string[]> Validate(PasswordRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (request.Length is < 4 or > 128)
        {
            errors[nameof(request.Length)] =
            [
                "Length must be between 4 and 128 characters."
            ];
        }

        if (!request.IncludeUppercase &&
            !request.IncludeLowercase &&
            !request.IncludeNumbers &&
            !request.IncludeSpecialCharacters)
        {
            errors["CharacterGroups"] =
            [
                "At least one character group must be enabled."
            ];
        }

        if (request.ExcludedCharacters?.Length > 100)
        {
            errors[nameof(request.ExcludedCharacters)] =
            [
                "ExcludedCharacters cannot contain more than 100 characters."
            ];
        }

        return errors;
    }
}
