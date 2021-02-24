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
        History.nodeInnovation = 0;
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
        simpleGenome.AddNodeGene(new NodeGene(NodeGene.TYPE.SENSOR, History.NodeInnovate()));
        simpleGenome.AddNodeGene(new NodeGene(NodeGene.TYPE.SENSOR, History.NodeInnovate()));
        simpleGenome.AddNodeGene(new NodeGene(NodeGene.TYPE.SENSOR, History.NodeInnovate()));
        simpleGenome.AddNodeGene(new NodeGene(NodeGene.TYPE.OUTPUT, History.NodeInnovate()));
        simpleGenome.AddConnectionGene(new ConnectionGene(1, 4, -1f, true, History.Innovate()));
        simpleGenome.AddConnectionGene(new ConnectionGene(2, 4, 1f, true, History.Innovate()));
        simpleGenome.AddConnectionGene(new ConnectionGene(3, 4, 1f, true, History.Innovate()));

        //simpleGenome.AddConnectionGene(new ConnectionGene(2, 3, 1f, true, History.Innovate()));
        
        Genome offspringGenome = Genome.Crossover(secondParent, firstParent);

        int numToRender = 80;
        int scale = 20;
        NEATRenderer renderer = new NEATRenderer();

        for (int i = 0; i < numToRender; i++) {
            renderer.DrawGenome(TetrisNEAT.OpenGenome("/new/"+i+".tetro"), new Vector3((scale+1) * (i+1), 0, 0), Vector2.one * scale);
        }

        // Un-comment the below line to conduct a test with the current evaluator after first removing the rendering instructions
        //StartCoroutine(SimpleTest());
    }

    IEnumerator SimpleTest() {
        NEATRenderer neatRenderer = new NEATRenderer();

        Evaluator eval = new Evaluator(200, simpleGenome);

        int drawI = 0;
        int scale = 20;
        int i = 0;
        while (i < 1000) {
            List<float> scores = new List<float>();
            foreach (Genome g in eval.genomes) {
                //scores.Add(eval.EvaluateGenome(g));
            }
            yield return new WaitForEndOfFrame();

            eval.Evaluate(scores.ToArray());
            if (i % 2 == 0) {
                //neatRenderer.DrawGenome(eval.GetFittestGenome(), new Vector3((scale+1) * (drawI+1), 0, 0), Vector2.one * scale);

                TextureFromANN grapher = GetComponent<TextureFromANN>();
                //grapher.DrawTexture(grapher.TextureFromNeuralNet(new NeuralNetwork(eval.GetFittestGenome()), 101, 101));
                
                drawI++;
            }
            yield return new WaitForEndOfFrame();


            print("Generation: " +i + "\nHighest fitness: "+eval.GetHighestFitness() + "\nNo. species: " + eval.GetSpeciesAmount());
            yield return new WaitForEndOfFrame();

            i++;
        }
        //neatRenderer.DrawGenome(eval.GetFittestGenome(), new Vector3((scale+1) * (drawI+1), 0, 0), Vector2.one * scale);
        //eval.EvaluateGenome(eval.GetFittestGenome(), true);
        //TetrisNEAT.SaveGenome(eval.GetFittestGenome(), "/TestGenome3.gen");
    }
}
