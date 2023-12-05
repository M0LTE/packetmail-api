using packetmail_api.Configuration;
using packetmail_api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<ConfigCheck>();
builder.Services.AddSingleton<BpqSessionManager>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<ServiceConfig>(builder.Configuration);

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();
app.Run();
