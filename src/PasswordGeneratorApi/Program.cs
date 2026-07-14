using PasswordGeneratorApi.Contracts;
using PasswordGeneratorApi.Services;
using PasswordGeneratorApi.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IPasswordGenerator, PasswordGenerator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => Results.Ok(new
{
    name = "Password Generator API",
    version = "1.0",
    documentation = "/swagger"
}))
.WithName("GetApiInformation")
.WithOpenApi();

app.MapGet("/password", (IPasswordGenerator generator) =>
{
    var options = new PasswordOptions();
    var result = generator.Generate(options);

    return Results.Ok(result);
})
.WithName("GenerateDefaultPassword")
.WithSummary("Generates a password using the default settings.")
.Produces<PasswordResponse>(StatusCodes.Status200OK)
.WithOpenApi();

app.MapPost("/password", (
    PasswordRequest request,
    IPasswordGenerator generator) =>
{
    var validationErrors = PasswordRequestValidator.Validate(request);

    if (validationErrors.Count > 0)
    {
        return Results.ValidationProblem(validationErrors);
    }

    var options = new PasswordOptions
    {
        Length = request.Length,
        IncludeUppercase = request.IncludeUppercase,
        IncludeLowercase = request.IncludeLowercase,
        IncludeNumbers = request.IncludeNumbers,
        IncludeSpecialCharacters = request.IncludeSpecialCharacters,
        ExcludedCharacters = request.ExcludedCharacters ?? string.Empty
    };

    var result = generator.Generate(options);

    return Results.Ok(result);
})
.WithName("GenerateCustomPassword")
.WithSummary("Generates a password using custom settings.")
.Produces<PasswordResponse>(StatusCodes.Status200OK)
.ProducesValidationProblem()
.WithOpenApi();

app.Run();

public partial class Program;
