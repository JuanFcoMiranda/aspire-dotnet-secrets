using Aspire.Hosting.ApplicationModel;

#pragma warning disable ASPIREINTERACTION001

namespace Aspire.Hosting;

public sealed class SigningTokenResource(
    string name,
    string issuer,
    IResourceBuilder<ParameterResource> signingKeyParameter) : Resource(name)
{
    public string Issuer { get; } = issuer;
    public IResourceBuilder<ParameterResource> SigningKeyParameter { get; } = signingKeyParameter;
}

#pragma warning restore ASPIREINTERACTION001
