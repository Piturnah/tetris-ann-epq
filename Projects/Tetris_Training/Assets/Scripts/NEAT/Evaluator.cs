using System;
using System.Collections;
using System.Collections.Generic;

public abstract class Evaluator {

    int populationSize;
    Dictionary<Genome, Species> speciesMap;
    Dictionary<Genome, float> scoreMap;
    List<Genome> genomes;
    List<Species> species;

    float c1 = 1f;
    float c2 = 1f;
    float c3 = 1f;
    float dt = 1f;

    void Evaluate() {
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

        // evaluate genomes and assign fitness
        foreach (Genome g in genomes) {
            Species s = speciesMap[g];

            float score = EvaluateGenome(g);
            float adjustedScore = score / speciesMap[g].members.Count;

            s.AddAdjustedFitness(adjustedScore);
            s.fitnessPop.Add(new FitnessGenome(g, adjustedScore));
            scoreMap.Add(g, adjustedScore);
        }

        // put best genomes from species into next generation
        foreach (Species s in species) {
            FitnessGenome fittestInSpecies = s.fitnessPop[0];
            //nextGenGenomes.add(fittestInSpecies.genome);
        }

        // breed genomes
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