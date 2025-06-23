using System;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Infrastructure;

public class KeyVaultService
{
    public SecretClient Client { get; }

    public KeyVaultService(string vaultUri)
    {
        Client = new SecretClient(new Uri(vaultUri), new DefaultAzureCredential());
    }

    public string? GetSecret(string name)
    {
        try
        {
            KeyVaultSecret secret = Client.GetSecret(name);
            return secret.Value;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
