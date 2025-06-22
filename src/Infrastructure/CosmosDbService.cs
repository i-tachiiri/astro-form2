using Microsoft.Azure.Cosmos;

namespace Infrastructure;

public class CosmosDbService
{
    public CosmosClient Client { get; }

    public CosmosDbService(string connectionString)
    {
        Client = new CosmosClient(connectionString);
    }
}