using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURE SETTINGS ---
builder.Services.Configure<PropVivoDatabaseSettings>(builder.Configuration.GetSection("PropVivoDatabaseSettings"));
builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("TwilioSettings"));

// --- 2. REGISTER DATABASE SERVICES ---
builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(sp.GetRequiredService<IOptions<PropVivoDatabaseSettings>>().Value.ConnectionString));
builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<PropVivoDatabaseSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});

// --- 3. REGISTER YOUR CUSTOM SERVICES ---
builder.Services.AddScoped<ICustomerService, CustomerService>();
// Note: You may need to add your CallLogService here if you created it
// builder.Services.AddScoped<ICallLogService, CallLogService>();

// --- 4. ADD FRAMEWORK SERVICES ---
builder.Services.AddControllers(); // This prepares controller services
builder.Services.AddSignalR().AddMessagePackProtocol();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// *** ADD THIS BLOCK TO CONFIGURE CORS ***
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policyBuilder =>
    {
        // This allows your frontend at localhost:5173 to connect
        policyBuilder.WithOrigins("http://localhost:5173", "https://*.netlify.app" )
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});


var app = builder.Build();

// --- 5. CONFIGURE THE HTTP REQUEST PIPELINE ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// This must come before UseAuthorization and MapControllers
app.UseCors("AllowReactApp");


app.UseAuthorization();

// *** THIS IS THE CRITICAL MISSING LINE ***
app.MapControllers(); // This activates the routes for your API controllers

// We also need to map the SignalR hub
app.MapHub<CallHub>("/callHub");

app.Run();