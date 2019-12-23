using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NEATTester : MonoBehaviour
{
    Genome firstParent = new Genome();
    Genome secondParent = new Genome();

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

        
        Genome offspringGenome = Genome.Crossover(secondParent, firstParent);
        NEATRenderer.DrawGenome(firstParent, Vector3.zero, Vector2.one * 5);

        Evaluator eval = new Evaluator(20, firstParent);

        int drawI = 0;
        for (int i = 0; i < 100; i++) {
            eval.Evaluate();
            
            if (i % 10 == 0) {
                NEATRenderer.DrawGenome(eval.GetFittestGenome(), new Vector3(6 * (drawI+1), 0, 0), Vector2.one * 5);
                drawI++;
            }
            print("Generation: " +i);
        }
    }
}
