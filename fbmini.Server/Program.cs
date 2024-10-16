using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using fbmini.Server.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

var keyVaultEndpoint = new Uri(Environment.GetEnvironmentVariable("VaultUri"));

builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddAuthentication(options => {
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
}).AddCookie(IdentityConstants.ApplicationScheme, options =>
{
    options.LoginPath = "/login";
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});
//options =>
//{
//    options.LoginPath = "login";
//    options.AccessDeniedPath = "form";
//    options.Cookie.SameSite = SameSiteMode.None;
//    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//}

builder.Services.AddDbContext<fbminiServerContext>(options =>
    options.UseSqlServer(builder.Configuration["AzureDbConnection"] ?? throw new InvalidOperationException("Connection string 'fbminiServerContext' not found.")));

builder.Services.AddIdentityCore<User>()
    .AddSignInManager()
    .AddEntityFrameworkStores<fbminiServerContext>()
    .AddApiEndpoints();

//builder.Services.AddAuthentication("UserAuthentication")
//    .AddScheme<AuthenticationSchemeOptions, UserAuthenticationHandler>("UserAuthentication", null);

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //using (var scope = app.Services.CreateScope())
    //{
    //    var db = scope.ServiceProvider.GetRequiredService<fbminiServerContext>();
    //    db.Database.EnsureDeleted();
    //    db.Database.Migrate();
    //}
}

app.UseStaticFiles();

app.UseHttpsRedirection();

//app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.MapFallbackToFile("/index.html");

//app.MapDBEndpoints();
//app.MapIdentityApi<User>();

app.Run();
