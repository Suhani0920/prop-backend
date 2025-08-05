using Microsoft.Extensions.Options;
using MongoDB.Driver;

public class CustomerService : ICustomerService
{
    private readonly IMongoCollection<Customer> _customersCollection;

    public CustomerService(IMongoDatabase database, IOptions<PropVivoDatabaseSettings> dbSettings)
    {
        _customersCollection = database.GetCollection<Customer>(
            dbSettings.Value.CustomersCollectionName);
    }

    // Add this new method to handle customer creation
    public async Task<Customer> CreateAsync(CreateCustomerDto customerDto)
    {
        var newCustomer = new Customer
        {
            Name = customerDto.Name,
            PhoneNumber = customerDto.PhoneNumber,
            Email = customerDto.Email
        };

        await _customersCollection.InsertOneAsync(newCustomer);
        return newCustomer;
    }

    public async Task<Customer?> GetByPhoneNumberAsync(string phoneNumber)
    {
        var filter = Builders<Customer>.Filter.Eq(c => c.PhoneNumber, phoneNumber);
        return await _customersCollection.Find(filter).FirstOrDefaultAsync();
    }
}