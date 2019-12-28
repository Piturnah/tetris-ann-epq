using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NEATTester : MonoBehaviour
{
    Genome firstParent = new Genome();
    Genome secondParent = new Genome();

    Genome simpleGenome = new Genome();

    void Start()
    {
        // FIRST PARENT
        firstParent.AddNodeGene(new NodeGene(NodeGene.TYPE.SENSOR, 1));
        firstParent.AddNodeGene(new NodeGene(NodeGene.TYPE.SENSOR, 2));
        firstParent.AddNodeGene(new NodeGene(NodeGene.TYPE.SENSOR, 3));

        firstParent.AddNodeGene(new NodeGene(NodeGene.TYPE.OUTPUT, 4));
        firstParent.AddNodeGene(new NodeGene(NodeGene.TYPE.HIDDEN, 5));


        // CONNECTIONS

        firstParent.AddConnectionGene(new ConnectionGene(1, 4, 1f, true, 1));
        firstParent.AddConnectionGene(new ConnectionGene(2, 4, 1f, false, 2));
        firstParent.AddConnectionGene(new ConnectionGene(3, 4, 1f, true, 3));
        firstParent.AddConnectionGene(new ConnectionGene(2, 5, 1f, true, 4));
        firstParent.AddConnectionGene(new ConnectionGene(5, 4, 1f, true, 5));
        firstParent.AddConnectionGene(new ConnectionGene(1, 5, 1f, true, 8));

        // SECOND PARENT
        secondParent.AddNodeGene(new NodeGene(NodeGene.TYPE.SENSOR, 1));
        secondParent.AddNodeGene(new NodeGene(NodeGene.TYPE.SENSOR, 2));
        secondParent.AddNodeGene(new NodeGene(NodeGene.TYPE.SENSOR, 3));

        secondParent.AddNodeGene(new NodeGene(NodeGene.TYPE.OUTPUT, 4));
        secondParent.AddNodeGene(new NodeGene(NodeGene.TYPE.HIDDEN, 5));
        secondParent.AddNodeGene(new NodeGene(NodeGene.TYPE.HIDDEN, 6));

        // CONNECTIONS

        secondParent.AddConnectionGene(new ConnectionGene(1, 4, 1f, true, 1));
        secondParent.AddConnectionGene(new ConnectionGene(2, 4, 1f, false, 2));
        secondParent.AddConnectionGene(new ConnectionGene(3, 4, 1f, true, 3));
        secondParent.AddConnectionGene(new ConnectionGene(2, 5, 1f, true, 4));
        secondParent.AddConnectionGene(new ConnectionGene(5, 4, 1f, false, 5));

        secondParent.AddConnectionGene(new ConnectionGene(5, 6, -1f, true, 6));
        secondParent.AddConnectionGene(new ConnectionGene(6, 4, 1f, true, 7));
        secondParent.AddConnectionGene(new ConnectionGene(3, 5, 1f, true, 9));
        secondParent.AddConnectionGene(new ConnectionGene(1, 6, 1f, true, 10));

        // SIMPLE GENOME
        simpleGenome.AddNodeGene(new NodeGene(NodeGene.TYPE.SENSOR, 1));
        simpleGenome.AddNodeGene(new NodeGene(NodeGene.TYPE.SENSOR, 2));
        simpleGenome.AddNodeGene(new NodeGene(NodeGene.TYPE.SENSOR, 3));
        simpleGenome.AddNodeGene(new NodeGene(NodeGene.TYPE.OUTPUT, 4));
        simpleGenome.AddConnectionGene(new ConnectionGene(1, 4, -1f, true, History.Innovate()));
        simpleGenome.AddConnectionGene(new ConnectionGene(2, 4, 1f, true, History.Innovate()));
        simpleGenome.AddConnectionGene(new ConnectionGene(3, 4, 1f, true, History.Innovate()));

        //simpleGenome.AddConnectionGene(new ConnectionGene(2, 3, 1f, true, History.Innovate()));
        
        Genome offspringGenome = Genome.Crossover(secondParent, firstParent);

        int numToRender = 1003;
        int scale = 20;

       //for (int i = 1002; i < numToRender; i++) {
       //    NEATRenderer.DrawGenome(TetrisNEAT.OpenGenome("/"+i+".tetro"), new Vector3((scale+1) * (i+1), 0, 0), Vector2.one * scale);
       //}

        SimpleTest();
    }

    void SimpleTest() {
        NEATRenderer neatRenderer = new NEATRenderer();

        Evaluator eval = new Evaluator(120, simpleGenome);

        int drawI = 0;
        int scale = 20;
        for (int i = 0; i < 1000; i++) {
            List<float> scores = new List<float>();
            foreach (Genome g in eval.genomes) {
                scores.Add(eval.EvaluateGenome(g));
            }

            eval.Evaluate(scores.ToArray());
            
            if (i % 10 == 0) {
                neatRenderer.DrawGenome(eval.GetFittestGenome(), new Vector3((scale+1) * (drawI+1), 0, 0), Vector2.one * scale);
                drawI++;
            }
            print("Generation: " +i + "\nHighest fitness: "+eval.GetHighestFitness() + "\nNo. species: " + eval.GetSpeciesAmount());
            
        }
    }
}
