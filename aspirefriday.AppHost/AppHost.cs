using Scalar.Aspire;

var builder = DistributedApplication.CreateBuilder(args);

var jwtSigningKey = builder.AddParameter(
    "jwt-signing-key",
    new Base64ParameterDefault(32),
    secret: true,
    persist: true);

var signingKey = builder.AddJwtSigningToken(
    name: "signing-key",
    issuer: "dotnet-user-jwts",
    signingKeyParameter: jwtSigningKey);

var apiService = builder.AddProject<Projects.aspirefriday_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithJwtToken(
        signingKey,
        commandName: "generate-jwt",
        displayName: "Generate User JWT",
        description: "Mints a signed user JWT bearer token using the configured static signing key.",
        defaultClaims: new Dictionary<string, JwtClaimDefault>
        {
            ["sub"] = new("dev-user", UserConfigurable: true, Label: "User ID", Description: "Value used for the sub claim."),
            ["type"] = new("user"),
            ["thing"] = new("1", UserConfigurable: true, Label: "Thing claim")
        })
    .WithJwtToken(
        signingKey,
        commandName: "generate-service-jwt",
        displayName: "Generate Service JWT",
        description: "Mints a signed service JWT bearer token using the configured static signing key.",
        defaultClaims: new Dictionary<string, JwtClaimDefault>
        {
            ["sub"] = new("dev-service", UserConfigurable: true, Label: "App ID", Description: "Application identifier used for the sub claim."),
            ["type"] = new("service"),
            ["thing"] = new("1", UserConfigurable: true, Label: "Thing claim")
        });

var scalar = builder.AddScalarApiReference()
    .WithApiReference(apiService);

builder.Build().Run();

internal sealed class Base64ParameterDefault(int byteLength) : Aspire.Hosting.ApplicationModel.ParameterDefault
{
    public int ByteLength { get; } = byteLength > 0
        ? byteLength
        : throw new ArgumentOutOfRangeException(nameof(byteLength));

    public override string GetDefaultValue()
    {
        return Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(ByteLength));
    }

    public override void WriteToManifest(Aspire.Hosting.Publishing.ManifestPublishingContext context)
    {
        context.Writer.WriteStartObject("generateBase64");
        context.Writer.WriteNumber("byteLength", ByteLength);
        context.Writer.WriteEndObject();
    }
}
