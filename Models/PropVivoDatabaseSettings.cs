public class PropVivoDatabaseSettings
{
    // The property names MUST match the keys in appsettings.json
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string CustomersCollectionName { get; set; } = null!;
}