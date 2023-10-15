using Microsoft.Extensions.Configuration;
using System.Security.Authentication;


namespace Backend.Data;



using MongoDB.Driver;

public class MongoDbContext
{
    public IMongoDatabase Database { get; }

    public MongoDbContext(IConfiguration configuration)
{
    var connectionString = configuration.GetSection("MongoSettings:Connection").Value;
    var settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
    settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
    var client = new MongoClient(settings);
    Database = client.GetDatabase(configuration.GetSection("MongoSettings:DatabaseName").Value);
}
}