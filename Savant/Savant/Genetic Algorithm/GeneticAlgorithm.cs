using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Savant
{
    class GeneticAlgorithm
    {
        public int Generation;
        public double ElitismChance;
        public double CrossOverChance;
        public double MutationChance;
        public double AverageFitness;
        public double HighestFitness;
        
        public List<Creature> NextGeneration;

        public GeneticAlgorithm(int CrossOverChance, int MutationChance)
        {
            this.CrossOverChance = CrossOverChance;
            this.ElitismChance = 100 - this.CrossOverChance;
            this.MutationChance = MutationChance;
        }

        public double Evolve(List<Creature> creature, Rectangle Bounds)
        {
            NextGeneration = new List<Creature>();

            CalculateFitness(creature);
            Elitism(creature);
            CrossOver(creature, Bounds);
            Mutation();
            CopyCreatures(creature);

            Generation++;

            NextGeneration.Clear();

            return AverageFitness;
        }

        public void Elitism(List<Creature> creature)
        {
            creature = creature.OrderByDescending(Creature => Creature.Fitness).ToList();
            int NrOfElites = (int)(creature.Count() * (double)(ElitismChance / 100));
            for (int i = 0; i < NrOfElites; i++)
                NextGeneration.Add(creature[i]);
        }

        public void CalculateFitness(List<Creature> creature)
        {
            HighestFitness = 0;
            AverageFitness = 0;
            foreach (Creature c in creature)
            {
                AverageFitness += c.Fitness;
                if (c.Fitness > HighestFitness)
                    HighestFitness = c.Fitness;
            }
            AverageFitness /= creature.Count();

            foreach (Creature c in creature)
            {
                if (HighestFitness == 0)
                {
                    c.ParentChance = 100;
                }
                else
                {
                    c.ParentChance = (c.Fitness / HighestFitness) * 100;
                }
            }
        }

        public Creature Selection()
        {
            NextGeneration = NextGeneration.OrderBy(Creature => Guid.NewGuid()).ToList();
            Random random = new Random(Guid.NewGuid().GetHashCode());
            int ParentTreshold = random.Next(0, 100);
            foreach (Creature c in NextGeneration)
            {
                if (c.ParentChance > ParentTreshold)
                {
                    return c;
                }
            }
            return null;
        }

        public void CrossOver(List<Creature> creature, Rectangle Bounds)
        {
            int NrOfCrossOver = (int)(creature.Count() * (double)(CrossOverChance / 100));
            for (int j = 0; j < NrOfCrossOver; j++)
            {
                Random random = new Random(Guid.NewGuid().GetHashCode());
                Creature ParentA = Selection();
                Creature ParentB = Selection();

                double[] ParentAWeights = ParentA.Brain.GetWeights();
                double[] ParentBWeights = ParentB.Brain.GetWeights();

                double[] ChildWeights = new double[ParentAWeights.Length];

                int CrossOverPoint = random.Next(0, ParentAWeights.Length);

                for (int i = 0; i < CrossOverPoint; i++)
                {
                    ChildWeights[i] = ParentAWeights[i];
                }
                for (int i = CrossOverPoint; i < ParentAWeights.Length; i++)
                {
                    ChildWeights[i] = ParentBWeights[i];
                }
                Creature Child = new Creature(Bounds);
                Child.Brain.SetWeights(ChildWeights);
                NextGeneration.Add(Child);
            }
        }

        public void Mutation()
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            foreach (Creature c in NextGeneration)
            {
                if (random.Next(0, 100) < MutationChance)
                {
                    int MutationPoint = random.Next(0, c.Brain.GetNrOfDendrites());
                    double[] Weights = c.Brain.GetWeights();
                    Weights[MutationPoint] = random.NextDouble();
                    c.Brain.SetWeights(Weights);
                }
            }
        }

        public void CopyCreatures(List<Creature> creature)
        {
            for (int i = 0; i < creature.Count(); i++)
            {
                creature[i] = NextGeneration[i];
                creature[i].Reset();
            }
        }
    }
}
