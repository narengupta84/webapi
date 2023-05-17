using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Models;
using WebApi.Repository;

namespace WebApi.Tests.Repository
{
    public class PokemonRepositoryTests
    {
        private async Task<DataContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var databaseContext = new DataContext(options);
            databaseContext.Database.EnsureCreated();
            if (await databaseContext.Pokemons.CountAsync() <= 0)
            {
                for (int i = 1; i <= 10; i++)
                {
                    databaseContext.Pokemons.Add(
                    new Pokemon()
                    {
                        Name = "Pikachu" + i.ToString(),
                        BirthDate = new DateTime(1984, 1, 2),
                        PokemonCategories = new List<PokemonCategory>()
                            {
                                new PokemonCategory { Category = new Category() { Name = "Electric"}}
                            },
                        Reviews = new List<Review>()
                            {
                                new Review { Title="Pikachu",Text = "Pickahu is the best pokemon, because it is electric", Rating = 5,
                                Reviewer = new Reviewer(){ FirstName = "Teddy", LastName = "Smith" } },
                                new Review { Title="Pikachu", Text = "Pickachu is the best a killing rocks", Rating = 5,
                                Reviewer = new Reviewer(){ FirstName = "Taylor", LastName = "Jones" } },
                                new Review { Title="Pikachu",Text = "Pickchu, pickachu, pikachu", Rating = 1,
                                Reviewer = new Reviewer(){ FirstName = "Jessica", LastName = "McGregor" } },
                            }
                    });
                    await databaseContext.SaveChangesAsync();
                }
            }
            return databaseContext;
        }

        [Fact]
        public async void PokemonRepository_GetPokemon_ReturnsPokemon()
        {
            //Arrange
            var name = "Pikachu1";
            var dbContext = await GetDatabaseContext();
            var pokemonRepository = new PokemonRepository(dbContext);

            //Act
            var result = pokemonRepository.GetPokemon(name);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Pokemon>();
        }
        [Fact]
        public async void PokemonRepository_GetPokemonRating_ReturnDecimalBetweenOneAndFive()
        {
            //Arrange
            var pokeId = 1;
            var dbContext = await GetDatabaseContext();
            var pokemonRepository = new PokemonRepository(dbContext);

            //Act
            var result = pokemonRepository.GetPokemonRating(pokeId);

            //Assert
            result.Should().NotBe(0);
            result.Should().BeInRange(1, 5);
        }
        [Fact]
        public async void PokemonRepository_PokemonExists_ReturnBoolean()
        {
            //Arrange
            var pokeId = 2;
            var dbContext = await GetDatabaseContext();
            var pokemonRepository = new PokemonRepository(dbContext);

            //Act
            var result = pokemonRepository.PokemonExists(pokeId);

            //Assert
            result.Should().Be(true);
        }
        [Fact]
        public async void PokemonRepository_DeletePokemon_ReturnBoolean()
        {
            //Arrange
            var name = "Pikachu1";
            var dbContext = await GetDatabaseContext();
            var pokemonRepository = new PokemonRepository(dbContext);

            //Act
            var pokemon = pokemonRepository.GetPokemon(name);
            var result = pokemonRepository.DeletePokemon(pokemon);

            //Assert
            result.Should().Be(true);
        }
    }
}
