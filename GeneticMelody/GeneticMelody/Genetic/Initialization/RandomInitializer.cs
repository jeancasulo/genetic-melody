﻿using GeneticMelody.Converter;
using GeneticMelody.Genetic.Domain;
using GeneticMelody.Genetic.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticMelody.Genetic.Initialization
{
    public class RandomInitializer : IInitializazer
    {
        public RandomInitializer(Melody melody)
        {
            BaseMelody = melody;
        }

        public Melody BaseMelody { get; set; }

        public Population Initialize()
        {
            var population = new Population();

            while (population.Individuals.Count < Population.Limit)
            {
                population.Individuals.Add(new Melody(GetMeasures(), BaseMelody.TempoMap));
            }

            return population;
        }

        private IList<Measure> GetMeasures()
        {
            var measures = new List<Measure>();

            var i = 0;
            while (measures.Count < BaseMelody.Measures.Count)
            {
                measures.Add(new Measure(RandomizeEvents(), i));
                i++;
            }

            return measures;
        }

        private IList<Event> RandomizeEvents()
        {
            var events = new List<Event>();
            var differentEvents = BaseMelody.Measures.SelectMany(m => m.Events)
                .GroupBy(e => e.Number)
                .ToList();

            var indexOfevent = 0;
            while (events.Count < Measure.SizeOfMeasure)
            {
                var index = ThreadSafeRandom.ThisThreadsRandom.Next(differentEvents.Count);
                var randomEvent = differentEvents[index].Key;

                if (indexOfevent == 0 && randomEvent == (int)RestOrTie.Tie)
                {
                    continue;
                }

                switch (randomEvent)
                {
                    case (int)RestOrTie.Rest:
                        events.Add(new Rest(randomEvent, events.Count));
                        break;

                    case (int)RestOrTie.Tie:
                        events.Add(new Tie(randomEvent, events.Count));
                        break;

                    default:
                        var note = new Melanchall.DryWetMidi.Smf.Interaction.Note((Melanchall.DryWetMidi.Common.SevenBitNumber)randomEvent);
                        events.Add(new Note(note.NoteName.ToString(), randomEvent, events.Count));
                        break;
                }

                indexOfevent++;
            }

            return events;
        }
    }
}