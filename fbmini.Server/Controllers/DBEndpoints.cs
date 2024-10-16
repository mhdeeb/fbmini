using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using fbmini.Server.Models;

namespace fbmini.Server.Controllers;

public static class DBEndpoints
{
    
    public static void MapDBEndpoints (this IEndpointRouteBuilder routes)
    {
        MapUser(routes);

        MapDB(routes);

        MapProducts(routes);
    }

    private static void MapUser(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/User").WithTags(nameof(User));

        group.MapPost("/", async (User user, fbminiServerContext context) =>
        {
            bool success;

            try
            {
                context.User.Add(user);
                await context.SaveChangesAsync();
                success = true;
            } catch (Exception) {
                success = false;
            }

            return (IResult) (success ? 
            TypedResults.Accepted("Creation Successful"):
            TypedResults.BadRequest("Failed to create user"));
        })
        .WithName("CreateUser")
        .WithOpenApi();

        //group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, fbminiServerContext db) =>
        //{
        //    var affected = await db.User
        //        .Where(model => model.ID == id)
        //        .ExecuteDeleteAsync();
        //    return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        //})
        //.WithName("DeleteUser")
        //.WithOpenApi();
    }

    private static void MapDB(IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/DB").WithTags(nameof(DB));

        group.MapGet("/", async (fbminiServerContext db) =>
        {
            return await db.DB.ToListAsync();
        })
        .WithName("GetAllDBs")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<DB>, NotFound>> (int id, fbminiServerContext db) =>
        {
            return await db.DB.AsNoTracking()
                .FirstOrDefaultAsync(model => model.ID == id)
                is DB model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetDBById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, DB dB, fbminiServerContext db) =>
        {
            var affected = await db.DB
                .Where(model => model.ID == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.ID, dB.ID)
                    .SetProperty(m => m.Name, dB.Name)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateDB")
        .WithOpenApi();

        group.MapPost("/", async (DB dB, fbminiServerContext db) =>
        {
            db.DB.Add(dB);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/DB/{dB.ID}", dB);
        })
        .WithName("CreateDB")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, fbminiServerContext db) =>
        {
            var affected = await db.DB
                .Where(model => model.ID == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteDB")
        .WithOpenApi();
    }

    private static void MapProducts(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Products").WithTags(nameof(Products));

        group.MapGet("/", async (fbminiServerContext db) =>
        {
            return await db.Products.ToListAsync();
        })
        .WithName("GetAllProducts")
        .WithOpenApi();
    }
}
