using CloudinaryDotNet.Actions;
using LibertyRustAcquiring.Data;
using LibertyRustAcquiring.Data.Extensions;
using LibertyRustAcquiring.Models.Entities;
using LibertyRustAcquiring.Settings;
using LibertyRustAcquiring.Utils;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly;

builder.WebHost.ConfigureKestrel((context, options) =>
{
    var kestrelConfig = context.Configuration.GetSection("Kestrel:Endpoints");

    var httpsConfig = kestrelConfig.GetSection("Https");
    var httpsUrl = httpsConfig["Url"];
    var certPath = httpsConfig["Certificate:Path"];
    var certPassword = httpsConfig["Certificate:Password"];

    if (!string.IsNullOrEmpty(httpsUrl) && !string.IsNullOrEmpty(certPath) && !string.IsNullOrEmpty(certPassword))
    {
        options.ListenAnyIP(new Uri(httpsUrl).Port, listenOptions =>
        {
            listenOptions.UseHttps(certPath, certPassword);
        });
    }
});

builder.Services.AddControllers();

builder.Services.AddSingleton<IPubKeyProvider, PubKeyProvider>();

builder.Services.AddScoped<IServerConnection, ServerConnection>();
builder.Services.AddScoped<IPhotoService, PhotoService>();

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
});

builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(opts =>
        opts.UseSqlite(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddCors(options =>

    options.AddDefaultPolicy(policy =>
    {
        //policy.WithOrigins(/*"http://test.liberty-rust.com.ua/"*//*"http://localhost:5173"*/);
        policy.AllowAnyOrigin();
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        //policy.AllowCredentials();
    })
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseMigration();

app.UseCors();

app.MapControllers();

async Task<string> ConnectRustRconAsync(ServerInfo serverInfo, string command)
{
    var serverUri = new Uri($"ws://{serverInfo.Hostname}:{serverInfo.RconPort}/{serverInfo.RconPassword}");

    using var ws = new ClientWebSocket();

    try
    {
        await ws.ConnectAsync(serverUri, CancellationToken.None);

        var payload = new
        {
            Identifier = 1,
            Message = command,
            Name = "WebRcon"
        };

        var jsonString = JsonSerializer.Serialize(payload);

        var bytesToSend = Encoding.UTF8.GetBytes(jsonString);

        await ws.SendAsync(
            new ArraySegment<byte>(bytesToSend),
            WebSocketMessageType.Text,
            endOfMessage: true,
            cancellationToken: CancellationToken.None
        );

        var buffer = new byte[4096];

        var result = await ws.ReceiveAsync(
            new ArraySegment<byte>(buffer),
            CancellationToken.None
        );

        var responseString = Encoding.UTF8.GetString(buffer, 0, result.Count);

        using var doc = JsonDocument.Parse(responseString);

        var message = doc.RootElement.GetProperty("Message").GetString();

        return message ?? "Нет поля 'Message' в ответе.";
    }
    catch (Exception ex)
    {
        return $"Failed to connect. {ex.Message}";
    }
}

app.MapPost("/send-command", async (IConfiguration configuration, RconCommandRequest request, IServerConnection server) =>
{
    if (string.IsNullOrWhiteSpace(request.Command))
    {
        return Results.BadRequest(new { error = "Поле 'command' отсутствует или пустое" });
    }

    var serverInfo = new ServerInfo
    {
        Hostname = configuration[$"{request.ServerName}:Ip"] ?? throw new ObjectIsNullException<ServerInfo>(),
        RconPort = configuration[$"{request.ServerName}:Port"] ?? throw new ObjectIsNullException<ServerInfo>(),
        RconPassword = configuration[$"{request.ServerName}:Password"] ?? throw new ObjectIsNullException<ServerInfo>(),
    };

    

    var response = await server.SendCommand(serverInfo, request.Command);

    return Results.Ok(response);
});

app.Run();