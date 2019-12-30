using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=System.Random;

public class Evaluator {

    int populationSize;
    Dictionary<Genome, Species> speciesMap;
    Dictionary<Genome, float> scoreMap;
    public List<Genome> genomes;
    List<Genome> nextGenGenomes;
    List<Species> species;

    const float c1 = 1.0f;
    const float c2 = 1.0f;
    const float c3 = 0.4f;
    float dt = 2f;
    const float _MUTATION_RATE = 0.8f;
    const float _ADD_CONONECTION_RATE = 0.05f;
    const float _ADD_NODE_RATE = 0.03f;
    const float _INTER_SPECIES_RATE = 0.01f;

    float highestScore;
    Genome fittestGenome;

    public float EvaluateGenome(Genome genome, bool display=false) { // change body for other evals
        float[][] xorIns = new float[4][] {
            new float [] {1, 0, 0},
            new float [] {1, 0, 1},
            new float [] {1, 1, 0},
            new float [] {1, 1, 1}
        }; 

        float[] xorOuts = new float[] {0, 1, 1, 0};

       NeuralNetwork xorNet = new NeuralNetwork(genome);
        float totCost = 0;

        for (int i = 0; i < 4; i++) {
            float nnResult = xorNet.GetNNResult(xorIns[i])[0];
            if (display) {
                ConsoleLogger.Log(nnResult.ToString());
            }

            totCost += Math.Abs(xorOuts[i] - nnResult);
        }

        return 4f - totCost;

    //    return genome.GetConnections().Count;
    }

    public Evaluator(int populationSize, Genome startingGenome) {
        this.populationSize = populationSize;
        genomes = new List<Genome>(populationSize);
        for (int i = 0; i < populationSize; i++) {
            genomes.Add(new Genome(startingGenome));
        }
        nextGenGenomes = new List<Genome>(populationSize);
        speciesMap = new Dictionary<Genome, Species>();
        scoreMap = new Dictionary<Genome, float>();
        species = new List<Species>();
    }

    public void Evaluate(float[] scores) {

        // reset everything
        foreach (Species s in species) {
            s.ResetSpecies(new System.Random());
        }

        scoreMap.Clear();
        speciesMap.Clear();
        nextGenGenomes.Clear();
        highestScore = float.MinValue;
        fittestGenome = null;

        // place genomes into species
        foreach (Genome g in genomes) {
            bool foundSpecies = false;
            foreach (Species s in species) {
                if (Genome.CompatibilityDist(g, s.mascot, c1, c2, c3) < dt) {
                    s.members.Add(g);
                    speciesMap.Add(g, s);
                    foundSpecies = true;
                    break;
                }
            }
            if (!foundSpecies) { // make new species
                Species newSpecies = new Species(g);
                species.Add(newSpecies);
                speciesMap.Add(g, newSpecies);
            }
        }



        // remove dead species
        foreach (Species s in species.ToList()) {
            if (s.members.Count == 0) {
                species.Remove(s);
            }
        }

        if (species.Count > 11) {
            dt += 0.04f;
            ConsoleLogger.Log("dt: " + dt.ToString());
        } else if (species.Count < 9) {
            dt -= 0.04f;
            ConsoleLogger.Log("dt: " + dt.ToString());
        }
        
        ConsoleLogger.Log("No. species: " + species.Count.ToString());

        // evaluate genomes and assign fitness
        foreach (Genome g in genomes) {
            Species s = speciesMap[g];

            float score = scores.ToList()[genomes.IndexOf(g)];
            float adjustedScore = score / speciesMap[g].members.Count;

            s.AddAdjustedFitness(adjustedScore);
            s.fitnessPop.Add(new FitnessGenome(g, adjustedScore));
            scoreMap.Add(g, adjustedScore);
            if (score > highestScore) {
                highestScore = score;
                fittestGenome = g;
            }
        }

        // put best genomes from species into next generation
        foreach (Species s in species) {
            // sort genomes by fitness
            s.fitnessPop.Sort(FitnessGenomeComparator.CompareTo);
            s.fitnessPop.Reverse();

            FitnessGenome fittestInSpecies = s.fitnessPop[0];
            nextGenGenomes.Add(fittestInSpecies.genome);
        }

        Random rand = new Random();

        // breed genomes
        while (nextGenGenomes.Count < populationSize) {
            Species s = GetRandomSpeciesBiasedAdjustedFitness(rand);

            Genome p1 = GetRandomGenomeBiasedAdjustedFitness(s, rand);

            if (rand.NextDouble() < _INTER_SPECIES_RATE) {
                s = GetRandomSpeciesBiasedAdjustedFitness(rand);
            }

            Genome p2 = GetRandomGenomeBiasedAdjustedFitness(s, rand);

            Genome child;
            if (scoreMap[p1] >= scoreMap[p2]) {
                child = Genome.Crossover(p1, p2);
            } else {
                child = Genome.Crossover(p2, p1);
            }
            if (rand.NextDouble() < _MUTATION_RATE) {
                child.ValueMutation();
            }
            if (rand.NextDouble() < _ADD_CONONECTION_RATE) {
                child.AddConnectionMutation();
            }
            if (rand.NextDouble() < _ADD_NODE_RATE) {
                child.AddNodeMutation();
            }
            nextGenGenomes.Add(child);

        }
        genomes = nextGenGenomes;
        nextGenGenomes = new List<Genome>();
    }

    Species GetRandomSpeciesBiasedAdjustedFitness(Random rand) {
        double completeWeight = 0.0;
        foreach (Species s in species) {
            completeWeight += s.totalAdjustedFitness;
        }
        double r = rand.NextDouble() * completeWeight;
        double countWeight = 0.0;
        foreach (Species s in species) {
            countWeight += s.totalAdjustedFitness;
            if (countWeight >= r) {
                return s;
            }
        }
        
        return species[rand.Next(species.Count)];
    }

    Genome GetRandomGenomeBiasedAdjustedFitness(Species selectFrom, Random rand) {
        double completeWeight = 0.0;
        foreach (FitnessGenome fg in selectFrom.fitnessPop) {
            completeWeight += fg.fitness;
        }
        double r = rand.NextDouble() * completeWeight;
        double countWeight = 0.0;
        foreach(FitnessGenome fg in  selectFrom.fitnessPop) {
            countWeight += fg.fitness;
            if (countWeight >= r) {
                return fg.genome;
            }
        }
        
        return selectFrom.fitnessPop[rand.Next(selectFrom.fitnessPop.Count)].genome;
    }

    public int GetSpeciesAmount() {
        return species.Count;
    }

    public float GetHighestFitness() {
        return highestScore;
    }

    public Genome GetFittestGenome() {
        return fittestGenome;
    }

    public class FitnessGenome {

        public Genome genome;
        public float fitness;

        public FitnessGenome(Genome genome, float fitness) {
            this.genome = genome;
            this.fitness = fitness;
        }
    }

    public class Species {

        public Genome mascot;
        public List<Genome> members;
        public List<FitnessGenome> fitnessPop;
        public float totalAdjustedFitness = 0f;

        public Species(Genome mascot) {
            this.mascot = mascot;
            this.members = new List<Genome>();
            this.members.Add(mascot);
            this.fitnessPop = new List<FitnessGenome>();
        }

        public void AddAdjustedFitness(float adjustedFitness) {
            this.totalAdjustedFitness += adjustedFitness;
        }

        public void ResetSpecies(Random r) {
            int newMascotIndex = r.Next(members.Count);
            this.mascot = members[newMascotIndex];
            members.Clear();
            fitnessPop.Clear();
            totalAdjustedFitness = 0f;
        }
    }

    public class FitnessGenomeComparator {

        public static int CompareTo(FitnessGenome one, FitnessGenome two) {
            if (one.fitness > two.fitness) {
                return 1;
            } else if (one.fitness < two.fitness) {
                return -1;
            }
            return 0;
        }
    }
}