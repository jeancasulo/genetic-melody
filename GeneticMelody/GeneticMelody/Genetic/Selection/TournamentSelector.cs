﻿using GeneticMelody.Genetic.Domain;
using GeneticMelody.Genetic.Util;
using System.Collections.Generic;
using System.Linq;

namespace GeneticMelody.Genetic.Selection
{
    public class TournamentSelector : ISelector
    {
        private const int NumberOfSelections = 4;

        public ICollection<Melody> Select(Population population)
        {
            var individuals = new List<Melody>();
            var indexes = new List<int>();

            for (int i = 0; i < NumberOfSelections; i++)
            {
                int index = ThreadSafeRandom.ThisThreadsRandom.Next(Population.Limit);

                while (indexes.Contains(index))
                {
                    index = ThreadSafeRandom.ThisThreadsRandom.Next(Population.Limit);
                }

                indexes.Add(index);
                individuals.Add(population.Individuals.ElementAt(index));
            }

            return individuals.OrderByDescending(i => i.Fitness).ToList();
        }
    }
}