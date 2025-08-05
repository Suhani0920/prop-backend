using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Customer
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("name")]
    public string? Name { get; set; }

    [BsonElement("email")]
    public string? Email { get; set; }

    [BsonElement("phoneNumber")]
    public string? PhoneNumber { get; set; }

    [BsonElement("address")]
    public string? Address { get; set; }
}










