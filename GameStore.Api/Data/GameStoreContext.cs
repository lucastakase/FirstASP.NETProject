using System;
using GameStore.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

public class GameStoreContext(DbContextOptions<GameStoreContext> options) : DbContext(options)
{
    public DbSet<Game> Games => Set<Game>();

    public DbSet<Genre> Genres => Set<Genre>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Genre>().HasData
        (
            new Genre { Id = 1, Name = "Action-adventure" },
            new Genre { Id = 3, Name = "Action RPG" },
            new Genre { Id = 4, Name = "Platformer" },
            new Genre { Id = 5, Name = "Strategy" },
            new Genre { Id = 6, Name = "Sports" },
            new Genre { Id = 7, Name = "Metroidvania" },
            new Genre { Id = 8, Name = "Horror" }
        );
    }
}
