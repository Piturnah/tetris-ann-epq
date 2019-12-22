using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public abstract class Evaluator {

    int populationSize;
    Dictionary<Genome, Species> speciesMap;
    Dictionary<Genome, float> scoreMap;
    List<Genome> genomes;
    List<Genome> nextGenGenomes;
    List<Species> species;

    const float c1 = 1f;
    const float c2 = 1f;
    const float c3 = 0.4f;
    const float dt = 10f;
    const float _MUTATION_RATE = 0.5f;
    const float _ADD_CONONECTION_RATE = 0.1f;
    const float _ADD_NODE_RATE = 0.1f;

    float highestScore;
    Genome fittestGenome;

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

    void Evaluate() {

        // reset everything
        foreach (Species s in species) {
            s.ResetSpecies(new Random());
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
        foreach (Species s in species) {
            if (s.members.Count == 0) {
                species.Remove(s);
            }
        }

        // evaluate genomes and assign fitness
        foreach (Genome g in genomes) {
            Species s = speciesMap[g];

            float score = EvaluateGenome(g);
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

            FitnessGenome fittestInSpecies = s.fitnessPop[0];
            nextGenGenomes.Add(fittestInSpecies.genome);
        }

        Random rand = new Random();

        // breed genomes
        while (nextGenGenomes.Count < populationSize) {
            Species s = GetRandomSpeciesBiasedAdjustedFitness(rand);

            Genome p1 = GetRandomGenomeBiasedAdjustedFitness(s, rand);
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
        throw new Exception("Couldn't find a new species");
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
        throw new Exception("Couldn't find a new genome");
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

    public abstract float EvaluateGenome(Genome genome);

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
}