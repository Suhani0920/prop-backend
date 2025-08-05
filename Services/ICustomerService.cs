// An interface defines a contract for what a class can do.
public interface ICustomerService
{
    Task<Customer?> GetByPhoneNumberAsync(string phoneNumber);
    Task<Customer> CreateAsync(CreateCustomerDto customerDto);
}