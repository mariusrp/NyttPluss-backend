using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;



namespace Backend.Models;
public class RssItem
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Title { get; set; }
    public string Link { get; set; }
    public string PubDate { get; set; }
    public string Description { get; set; }
    public string? ImageUrl { get; set; }
}