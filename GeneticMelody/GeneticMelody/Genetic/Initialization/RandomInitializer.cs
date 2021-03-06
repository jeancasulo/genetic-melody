﻿using GeneticMelody.Converter;
using GeneticMelody.Genetic.Domain;
using GeneticMelody.Genetic.Util;
using GeneticMelody.Util;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.MusicTheory;
using Melanchall.DryWetMidi.Smf.Interaction;
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
            var population = new Population(1);

            while (population.Individuals.Count < Population.Limit)
            {
                population.Individuals.Add(new Melody(GetMeasures(population.Individuals.Count), BaseMelody.TimeMap));
            }

            return population;
        }

        private IList<Measure> GetMeasures(int indivudualNumber)
        {
            var measures = new List<Measure>();

            var i = 0;
            while (measures.Count < BaseMelody.Measures.Count)
            {
                measures.Add(indivudualNumber < GeneticMelodyConstants.POPULATION_COUNT_WITH_ORIGINAl_TIES ? GetNewMeasureWithOriginalTies(i) : GetNewMeasure(i));
                i++;
            }

            return measures;
        }

        private Measure GetNewMeasure(int order)
        {
            var events = new List<Event>();
            var geneticEventsManager = Singleton<GeneticEventsManager>.Instance();

            var indexOfevent = 0;
            while (events.Count < BaseMelody.SizeOfMeasure)
            {
                var randomEvent = geneticEventsManager.RandomEvent();

                // Ensure that ties are not generated in first position
                if (indexOfevent == 0 && (randomEvent == (int)RestOrTie.Tie))
                {
                    continue;
                }

                // Ensure that ties are not generated after rests
                if (events.Any() && events.Last().Number == (int)RestOrTie.Rest && randomEvent == (int)RestOrTie.Tie)
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

            return new Measure(events, order);
        }

        private Measure GetNewMeasureWithOriginalTies(int order)
        {
            var events = new List<Event>();
            var geneticEventsManager = Singleton<GeneticEventsManager>.Instance();

            var indexOfevent = 0;
            while (events.Count < BaseMelody.SizeOfMeasure)
            {
                var randomEvent = geneticEventsManager.RandomRestOrNote();

                if (BaseMelody.Measures[order].Events[indexOfevent].Number == (int)RestOrTie.Tie)
                {
                    // Ensure that ties are not generated after rests
                    if (events.Last().Number == (int)RestOrTie.Rest)
                    {
                        events.Remove(events.Last());
                        var note = geneticEventsManager.RandomNote();
                        var name = NoteUtilities.GetNoteName((SevenBitNumber)note).ToString();
                        events.Add(new Note(name, note, events.Count));
                    }

                    events.Add(new Tie((int)RestOrTie.Tie, events.Count));
                }
                else
                {
                    // Ensure that ties are not generated in first position
                    if (indexOfevent == 0 && (randomEvent == (int)RestOrTie.Tie))
                    {
                        continue;
                    }

                    // Ensure that ties are not generated after rests
                    if (events.Any() && events.Last().Number == (int)RestOrTie.Rest && randomEvent == (int)RestOrTie.Tie)
                    {
                        continue;
                    }

                    switch (randomEvent)
                    {
                        case (int)RestOrTie.Rest:
                            events.Add(new Rest(randomEvent, events.Count));
                            break;

                        default:
                            var name = NoteUtilities.GetNoteName((SevenBitNumber)randomEvent).ToString();
                            events.Add(new Note(name, randomEvent, events.Count));
                            break;
                    }
                }

                indexOfevent++;
            }

            return new Measure(events, order);
        }
    }
}