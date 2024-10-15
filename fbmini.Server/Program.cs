using Microsoft.EntityFrameworkCore;
using fbmini.Server.Controllers;
using Azure.Identity;
using fbmini.Server.Models;

var builder = WebApplication.CreateBuilder(args);

var keyVaultEndpoint = new Uri(Environment.GetEnvironmentVariable("VaultUri"));

builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());

builder.Services.AddDbContext<fbminiServerContext>(options =>
    options.UseSqlServer(builder.Configuration["AzureDbConnection"] ?? throw new InvalidOperationException("Connection string 'fbminiServerContext' not found.")));

builder.Services.AddControllers();

var app = builder.Build();

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.MapDBEndpoints();

app.Run();
