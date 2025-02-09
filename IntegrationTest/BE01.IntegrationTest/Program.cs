using UiRtc.Public;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddUiRealTimeCommunicator();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowed(origin => true); // Allow all origins
    });
});

// Add services to the container.

var app = builder.Build();


// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();
app.UseCors();

app.UseUiRealTimeCommunicator();
app.Run();
