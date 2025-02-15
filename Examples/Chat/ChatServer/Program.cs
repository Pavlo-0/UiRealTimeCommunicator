using Chat.DataAccess;
using Chat.DataAccess.Models;
using UiRtc.Public;

var builder = WebApplication.CreateBuilder(args);
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
builder.Services.AddUiRealTimeCommunicator();
builder.Services.AddSingleton<IRepository<MessageRecord>>(new InMemoryRepository<MessageRecord>());
builder.Services.AddSingleton<IRepository<UserRecord>>(new InMemoryRepository<UserRecord>());

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors();
app.UseUiRealTimeCommunicator();
app.Run();
