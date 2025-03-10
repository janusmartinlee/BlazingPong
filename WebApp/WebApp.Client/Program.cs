using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PongEngine;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped<GameService>();

await builder.Build().RunAsync();
