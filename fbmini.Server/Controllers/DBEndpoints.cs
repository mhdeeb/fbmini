using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using fbmini.Server.Data;
namespace fbmini.Server.Controllers;

public static class DBEndpoints
{
    public static void MapDBEndpoints (this IEndpointRouteBuilder routes)
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
            return TypedResults.Created($"/api/DB/{dB.ID}",dB);
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

        group = routes.MapGroup("/api/Products").WithTags(nameof(Products));

        group.MapGet("/", async (fbminiServerContext db) =>
        {
            return await db.Products.ToListAsync();
        })
        .WithName("GetAllProducts")
        .WithOpenApi();
    }
}
