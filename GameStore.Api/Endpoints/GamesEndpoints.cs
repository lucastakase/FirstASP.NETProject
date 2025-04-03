using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string GetGamesRoute = "GetGame";

    private static readonly List<GameSummaryDto> games = 
    [
    new GameSummaryDto(1, "The Legend of Zelda: Breath of the Wild", "Action-adventure", 59.99m, new DateOnly(2017, 3, 3)),
    new GameSummaryDto(2, "Super Mario Odyssey", "Platformer", 59.99m, new DateOnly(2017, 10, 27)),
    new GameSummaryDto(3, "The Witcher 3: Wild Hunt", "Action RPG", 39.99m, new DateOnly(2015, 5, 19)),
    new GameSummaryDto(4, "Dark Souls III", "Action RPG", 39.99m, new DateOnly(2016, 3, 24)),
    new GameSummaryDto(5, "Hollow Knight", "Metroidvania", 14.99m, new DateOnly(2017, 2, 24))
    ];

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games")
            .WithParameterValidation();

        //GET /games
        group.MapGet("/", () => games);

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
        group.MapPut("/{id}", (int id, UpdateGameDto UpdateGameDto) =>
        {
            var index = games.FindIndex(game => game.Id == id);

            // Check if the game exists
            if (index == -1)
            {
                return Results.NotFound();
            }
            ;

            games[index] = new GameSummaryDto
            (
                id,
                UpdateGameDto.Name,
                UpdateGameDto.Genre,
                UpdateGameDto.Price,
                UpdateGameDto.ReleaseDate
            );

            return Results.NoContent();
        })
        .WithParameterValidation();

        //DELETE /games/1
        group.MapDelete("/{id}", (int id) =>
        {
            var index = games.FindIndex(game => game.Id == id);
            games.RemoveAt(index);
            return Results.NoContent();
        });

        return group;

    }
}
