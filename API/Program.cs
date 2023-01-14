using System.Net.Http.Headers;
using API.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<MessageHandler>();
builder.Services.AddHttpClient<MessageHandler>(client =>
{
    client.BaseAddress = new Uri("https://graph.facebook.com/v15.0/");
    client.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", builder.Configuration["FBToken"]);
    client.DefaultRequestHeaders.Add("User-Agent", "C# App");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
