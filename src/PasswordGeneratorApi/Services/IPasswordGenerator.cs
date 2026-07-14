using PasswordGeneratorApi.Contracts;

namespace PasswordGeneratorApi.Services;

public interface IPasswordGenerator
{
    PasswordResponse Generate(PasswordOptions options);
}
