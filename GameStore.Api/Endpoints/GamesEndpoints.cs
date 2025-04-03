using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string GetGamesRoute = "GetGame";

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games")
            .WithParameterValidation();

        //GET /games
        group.MapGet("/", (GameStoreContext dbContext) => dbContext.Games
            .Include(game => game.Genre)  
            .Select(game => game.ToGameSummaryDto())
            .AsNoTracking());

        //GET /games/1
        group.MapGet("/{id}", (int id, GameStoreContext dbContext) =>
        {
            Game? game = dbContext.Games.Find(id);

            return game is null ? Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
        })
        .WithName(GetGamesRoute);

        //POST /games
        group.MapPost("/", (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            if(string.IsNullOrEmpty(newGame.Name))
            {
                return Results.BadRequest("Name and Genre are required.");
            }
            Game game = newGame.ToEntity();

            dbContext.Games.Add(game);
            dbContext.SaveChanges();


            return Results.CreatedAtRoute(GetGamesRoute, new { id = game.Id }, game.ToGameDetailsDto());
        })
        .WithParameterValidation();

        //PUT /games
        group.MapPut("/{id}", (int id, UpdateGameDto UpdateGameDto, GameStoreContext dbContext) =>
        {
            var existingGame = dbContext.Games.Find(id);

            // Check if the game exists
            if (existingGame is null)
            {
                return Results.NotFound();
            }
            
            dbContext.Entry(existingGame)
                .CurrentValues
                .SetValues(UpdateGameDto.ToEntity(id));
            
            dbContext.SaveChanges();

            return Results.NoContent();
        })
        .WithParameterValidation();

        //DELETE /games/1
        group.MapDelete("/{id}", (int id, GameStoreContext dbContext) =>
        {
            dbContext.Games
                .Where(game => game.Id == id)
                .ExecuteDelete();

            return Results.NoContent();
        });

        return group;

    }
}
