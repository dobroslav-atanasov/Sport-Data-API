﻿namespace SportHub.Data.Seeders.OlympicGamesDb;

using System;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using SportHub.Data.Models.Entities.OlympicGames;
using SportHub.Data.Models.Entities.OlympicGames.Enumerations;
using SportHub.Data.Repositories;
using SportHub.Data.Seeders.Interfaces;

public class MedalsSeeder : ISeeder
{
    public async Task SeedAsync(IServiceProvider services)
    {
        var repository = services.GetService<OlympicGamesRepository<Medal>>();

        var types = new List<MedalTypeEnum>
        {
            MedalTypeEnum.Gold,
            MedalTypeEnum.Silver,
            MedalTypeEnum.Bronze,
            MedalTypeEnum.None
        };

        foreach (var type in types)
        {
            var dbType = await repository.GetAsync(x => x.Name == type.ToString());
            if (dbType == null)
            {
                var medal = new Medal
                {
                    Name = type.ToString()
                };

                await repository.AddAsync(medal);
                await repository.SaveChangesAsync();
            }
        }
    }
}