using Shared;
using Shared.Models;
using webapi;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
            builder =>
            {
                builder.WithOrigins("http://192.168.0.131:8010")
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .SetIsOriginAllowed((host) => true)
                       .AllowCredentials();
            }));

builder.Services.AddSignalR(o =>
{
    o.EnableDetailedErrors = true;
});
builder.Services.AddSingleton<IChatRepository, ChatRepository>();
// creates an instance of the ISignalRProcessor that can be handled by SignalR
//builder.Services.AddSingleton<ISignalRProcessor, PulsarService>();

// starts the IHostedService, which creates the ActorSystem and actors
//builder.Services.AddHostedService<PulsarService>(sp =>
    //(PulsarService)sp.GetRequiredService<ISignalRProcessor>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("CorsPolicy");
app.MapHub<PulsarHub>("chathub");
app.UseStaticFiles();

app.MapFallbackToFile("index.html");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
