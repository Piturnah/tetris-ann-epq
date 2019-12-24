using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TetrisNEAT : MonoBehaviour
{
    [SerializeField]
    public GameObject agent;

    Genome startingGenome = new Genome();
    Evaluator eval;

    int popSize = 20;

    int popIteration = 0;

    bool agentsPlaying = false;
    GameObject runningAgent;
    List<int> scores = new List<int>();

    void Start()
    {
        Time.timeScale = 10;
        eval = new Evaluator(popSize, startingGenome);
    }

    private void Update() {
        if (!agentsPlaying) {
            agentsPlaying = true;
            if (popIteration < popSize) { // still in same generation
                runningAgent = MakeNewAgent(eval.genomes[popIteration]);
            }
        }
    }

    void TakeScore(int score) {
        scores.Add(score);
        Destroy(runningAgent);
        popIteration++;
        if (popIteration == popSize) {
            popIteration = 0;
            eval.Evaluate(scores.ToArray().Select(x => (float)x).ToArray()); // evaluate and breed current genomes
        }
        agentsPlaying = false;
    }

    public GameObject MakeNewAgent(Genome genome) {
        GameObject newAgent = Instantiate(agent, Vector3.zero, Quaternion.identity);
        newAgent.GetComponent<TetrisAgent>().neuralNet = new NeuralNetwork(genome);

        newAgent.GetComponent<TetrisAgent>().died += TakeScore;

        return newAgent;
    }

    private void Awake() {

        // add initial sensors for board
        for (int y = 1; y < 21; y++) {
            for (int x = 1; x < 11; x++) {
                startingGenome.AddNodeGene(new NodeGene(NodeGene.TYPE.SENSOR, (y - 1) * 10 + x));
            }
        }
        // add initial sensors for current tetromino
        for (int y = 1; y < 5; y++) {
            for (int x = 1; x < 5; x++) {
                startingGenome.AddNodeGene(new NodeGene(NodeGene.TYPE.SENSOR, (y - 1) * 4 + x + 200));
            }
        }
        // add initial sensor for next tetromino
        startingGenome.AddNodeGene(new NodeGene(NodeGene.TYPE.SENSOR, 217));
        // add tetromino position sensor
        startingGenome.AddNodeGene(new NodeGene(NodeGene.TYPE.SENSOR, 218));
        startingGenome.AddNodeGene(new NodeGene(NodeGene.TYPE.SENSOR, 219));
        // add initial BIAS SENSOR - SHOULD ALWAYS BE SET TO 1
        startingGenome.AddNodeGene(new NodeGene(NodeGene.TYPE.SENSOR, 220));

        //add initial out nodes
        // 221 - left
        // 222 - down
        // 223 - right
        // 224 - a
        // 225 - b
        for (int i = 221; i < 226; i++) {
            startingGenome.AddNodeGene(new NodeGene(NodeGene.TYPE.OUTPUT, i));
        }

        // add initial connection from bias to down button
        startingGenome.AddConnectionGene(new ConnectionGene(220, 222, 1f, true, History.Innovate()));
    }
}
