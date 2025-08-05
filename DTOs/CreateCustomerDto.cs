using System.ComponentModel.DataAnnotations;

public class CreateCustomerDto
{
    [Required] // Marks the property as required
    public string? Name { get; set; }

    [Required]
    public string? PhoneNumber { get; set; }

    [EmailAddress] // Validates that the string is a valid email format
    public string? Email { get; set; }
}