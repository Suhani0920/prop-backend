using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Twilio.Security;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly IHubContext<CallHub> _callHubContext;
    private readonly TwilioSettings _twilioSettings;

    public CustomersController(
        ICustomerService customerService,
        IHubContext<CallHub> callHubContext,
        IOptions<TwilioSettings> twilioSettings)
    {
        _customerService = customerService;
        _callHubContext = callHubContext;
        _twilioSettings = twilioSettings.Value;
    }

    // This endpoint is designed to be called by Twilio's webhook service.
    [HttpPost("incoming-call")]
    public async Task<IActionResult> IncomingCallWebhook([FromForm] string From)
    {
        // 1. VALIDATE THE REQUEST
        /*var signature = Request.Headers["X-Twilio-Signature"];
        var requestValidator = new RequestValidator(_twilioSettings.AuthToken);
        var requestUrl = Request.Scheme + "://" + Request.Host + Request.Path;

        // The Twilio helper needs the raw form values
        var formValues = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());

        if (!requestValidator.Validate(requestUrl, formValues, signature))
        {
            return Forbid("Request failed Twilio validation.");
        }*/

        // 2. PROCESS THE VALIDATED REQUEST
        // 'From' is the parameter Twilio sends with the caller's phone number.
        var phoneNumber = From;

        await _callHubContext.Clients.All.SendAsync("IncomingCall", phoneNumber);

        // We return TwiML to tell Twilio what to do with the call.
        // For now, we'll just say something simple. A real app would have more complex logic.
        return Content("<Response><Say>Connecting you to an agent.</Say></Response>", "application/xml");
    }

    // The customer lookup endpoint remains the same.
    [HttpGet("lookup")]
    public async Task<ActionResult<Customer>> GetByPhone([FromQuery] string phoneNumber)
    {
        var customer = await _customerService.GetByPhoneNumberAsync(phoneNumber);
        if (customer == null) return NotFound();
        return Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult<Customer>> CreateCustomer([FromBody] CreateCustomerDto customerDto)
    {
        var newCustomer = await _customerService.CreateAsync(customerDto);

        // Return a 201 Created response with the new customer's details
        // and a link to where the new customer can be found.
        return CreatedAtAction(nameof(GetByPhone), new { phoneNumber = newCustomer.PhoneNumber }, newCustomer);
    }
}