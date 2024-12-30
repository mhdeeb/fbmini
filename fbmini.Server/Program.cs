using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using fbmini.Server.Models;
using fbmini.Server.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace fbmini.Server
{
    public class Program
    {
        static void ConfigBuilder(WebApplicationBuilder builder)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                builder.Services.AddDbContext<fbminiServerContext>(options =>
                    options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=fbmini-database")
                );

                builder.Services.AddSwaggerGen();

                builder.Services.AddEndpointsApiExplorer();
            }
            else
            {
                builder.Configuration.AddAzureKeyVault(
                    new Uri(Environment.GetEnvironmentVariable("VaultUri") ?? throw new InvalidOperationException("Environment Variable VaultUri not found.")),
                    new DefaultAzureCredential()
                );

                builder.Services.AddDbContext<fbminiServerContext>(options =>
                    options.UseSqlServer(
                        builder.Configuration["AzureDbConnection"] ?? throw new InvalidOperationException("Connection string 'fbminiServerContext' not found.")
                    )
                );
            }

            builder.Services.AddControllers();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
            }).AddCookie(IdentityConstants.ApplicationScheme, options =>
            {
                options.LoginPath = "/login";
                //options.AccessDeniedPath = "form";
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("FileAccessPolicy", policy =>
                    policy.Requirements.Add(new FileAccessRequirement()));
            });

            builder.Services.AddSingleton<IAuthorizationHandler, FileAuthorizationHandler>();

            builder.Services.AddIdentityCore<UserModel>()
                .AddRoles<IdentityRole>()
                .AddSignInManager()
                .AddEntityFrameworkStores<fbminiServerContext>();

            builder.Services.AddAuthorization();
        }

        static async Task ConfigAppOnce(WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            // ========================================================================
            // Database Migration
            var dbContext = scope.ServiceProvider.GetRequiredService<fbminiServerContext>();

            try
            {
                //dbContext.Database.EnsureDeleted();
                dbContext.Database.Migrate();
                Console.WriteLine("Database migrated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during migration: {ex.Message}");
            }

            // ========================================================================
            // Create Roles
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var roles = new[] { "Admin", "Manager", "User" };

            foreach (var role in roles)
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));

            // ========================================================================
            // Create Admin user

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserModel>>();

            var user = new UserModel { UserName = "admin@admin.com", Email = "admin@admin.com" };
            var result = await userManager.CreateAsync(user, "Admin@0");
            await userManager.AddToRoleAsync(user, roles[0]);
            if (result.Succeeded)
            {
                var userData = new UserDataModel { UserId = user.Id };
                dbContext.UserData.Add(userData);
                await dbContext.SaveChangesAsync();
                user = await dbContext.Users.FindAsync(user.Id);
                user!.UserDataId = userData.Id;
                dbContext.Users.Update(user);
                await dbContext.SaveChangesAsync();
            }
        }

        static async Task ConfigApp(WebApplication app)
        {
            //await ConfigAppOnce(app);

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                // Production specific
            }

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.MapFallbackToFile("/index.html");
        }

        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigBuilder(builder);

            var app = builder.Build();

            await ConfigApp(app);

            app.Run();
        }
    }
}