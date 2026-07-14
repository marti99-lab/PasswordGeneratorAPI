# Password Generator API

This is a small ASP.NET Core project that generates random passwords through a REST API.

I built it as a compact example of how I structure a simple .NET application. The project is intentionally not overloaded with features. The main goal was to keep the API easy to understand while still using a few things that are important in real applications, such as dependency injection, request validation, separate contracts, a service interface and automated tests.

## What the API can do

The API provides two endpoints:

- `GET /password` generates a password with the default settings.
- `POST /password` generates a password with custom settings.

The custom request supports:

- Password length
- Uppercase letters
- Lowercase letters
- Numbers
- Special characters
- Characters that should be excluded

The response also contains a basic strength rating and the size of the character pool used for generation.

## Technology used

- .NET 8
- ASP.NET Core Minimal API
- Swagger / OpenAPI
- xUnit
- `RandomNumberGenerator` from `System.Security.Cryptography`

## Why `RandomNumberGenerator` is used

The project does not use the normal `Random` class for creating passwords.

`Random` is useful for many general programming tasks, but it is not designed for security-related values. This project uses `RandomNumberGenerator.GetInt32` instead. It provides cryptographically secure random numbers and is therefore a better choice for password generation.

## Project structure

```text
PasswordGeneratorApi
├── src
│   └── PasswordGeneratorApi
│       ├── Contracts
│       ├── Services
│       ├── Validation
│       └── Program.cs
├── tests
│   └── PasswordGeneratorApi.Tests
├── PasswordGeneratorApi.sln
└── README.md
```

### Contracts

The `Contracts` folder contains the request and response models used by the API.

`PasswordRequest` describes the values that can be sent to the `POST /password` endpoint.

`PasswordResponse` describes the result returned by both password endpoints.

Keeping these models separate from the generator itself makes it easier to change the API contract without mixing it with the internal generation logic.

### Services

`IPasswordGenerator` is the interface used by the API endpoint.

`PasswordGenerator` contains the actual password generation logic.

The implementation is registered through dependency injection in `Program.cs`:

```csharp
builder.Services.AddSingleton<IPasswordGenerator, PasswordGenerator>();
```

The endpoint depends on the interface instead of creating the generator directly. This keeps the endpoint small and makes the service easier to replace or test.

`PasswordStrengthCalculator` calculates a simple rating based on password length and the number of possible characters. The result is an estimate and should not be treated as a full security analysis.

### Validation

`PasswordRequestValidator` checks incoming requests before a password is generated.

The current rules are:

- Length must be between 4 and 128 characters.
- At least one character group must be enabled.
- The exclusion list cannot contain more than 100 characters.

Invalid requests return HTTP status `400 Bad Request` together with a validation response.

## Running the project

Requirements:

- .NET 8 SDK

Clone the repository and restore the packages:

```bash
dotnet restore
```

Start the API:

```bash
dotnet run --project src/PasswordGeneratorApi
```

Swagger is available in the development environment at:

```text
http://localhost:5180/swagger
```

The exact port can be different when it is changed in `launchSettings.json` or by the development environment.

## Example requests

### Generate a password with default settings

```http
GET /password
```

Example response:

```json
{
  "password": "generated-password",
  "length": 16,
  "strength": "Very strong",
  "characterPoolSize": 88
}
```

The password shown above is only an example. A new value is generated for every request.

### Generate a custom password

```http
POST /password
Content-Type: application/json
```

```json
{
  "length": 20,
  "includeUppercase": true,
  "includeLowercase": true,
  "includeNumbers": true,
  "includeSpecialCharacters": true,
  "excludedCharacters": "0O1l"
}
```

This request creates a 20-character password and prevents characters such as zero, capital O, one and lowercase L from being used. This can be useful when a password has to be read or entered manually.

## Running the tests

Run all tests from the solution folder:

```bash
dotnet test
```

The tests check that:

- The requested password length is respected.
- Excluded characters are not used.
- Disabled character groups are not used.
- Invalid generator options are rejected.
- The strength calculator returns the expected ratings.

## Possible improvements

The project is deliberately small, but a few useful additions would be possible:

- Rate limiting
- Docker support
- Integration tests for the HTTP endpoints
- Configurable character sets
- Password strength checks based on common password lists
- A small frontend client

For the current scope, I wanted to keep the project focused on one task and make the implementation easy to follow.
