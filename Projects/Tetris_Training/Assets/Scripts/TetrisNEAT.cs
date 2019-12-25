using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class TetrisNEAT : MonoBehaviour
{
    [SerializeField] public GameObject agent;
    [SerializeField] Text genText;
    [SerializeField] Text genomeText;
    [SerializeField] Text highScoreText;

    Genome startingGenome = new Genome();
    Evaluator eval;

    int popSize = 100;
    int batchSize = 50;
    int batched = 0;
    int generation = 0;

    int popIteration = 0;
    int highScore = 0;

    bool agentsPlaying = false;
    bool oneByOne = false;
    GameObject runningAgent;
    List<int> scores = new List<int>();

    void Start()
    {
        Time.timeScale = 2;
        eval = new Evaluator(popSize, startingGenome);
        genText.text = "Generation: " + generation.ToString("0000");
        if (!oneByOne) {
            genomeText.text = "No. species: " + eval.GetSpeciesAmount().ToString("0000");
        }
    }

    private void Update() {
        if (oneByOne) {
            if (!agentsPlaying) {
                agentsPlaying = true;
                if (popIteration < popSize) { // still in same generation
                    runningAgent = MakeNewAgent(eval.genomes[popIteration]);

                    genomeText.text = "Genome: " + popIteration.ToString("0000");
                }
            }
        } else {
            if (!agentsPlaying) {
                agentsPlaying = true;
                for (int i = 0; i < batchSize; i++) {
                    GameObject newAgent = MakeNewAgent(eval.genomes[i + batched * batchSize]);
                    if (i == 0) {
                        newAgent.GetComponent<EngineUI>().enabled = true;
                    }
                }
                
            }
        }
    }

    void TakeScore(int score, GameObject deadObj) {
        scores.Add(score);
        Destroy(deadObj);
        popIteration++;
        if (popIteration == batchSize) {
            batched++;
            agentsPlaying = false;
            
            genomeText.text = "No. species: " + eval.GetSpeciesAmount().ToString("0000");

            if (popIteration * batched == popSize) {
                batched = 0;
                eval.Evaluate(scores.ToArray().Select(x => (float)x).ToArray()); // evaluate and breed current genomes
            
                SaveGenome(eval.GetFittestGenome(), "/" + generation.ToString() + ".tetro");

                generation++;
            }
            popIteration = 0;

            genText.text = "Generation: " + generation.ToString("0000");
        }
        if (oneByOne) {
            agentsPlaying = false;
        }
        

        if (score > highScore) {
            highScore = score;
            UpdateHighScore();
        }
    }

    void UpdateHighScore() {
        highScoreText.text = "Highscore: " + highScore.ToString("0000000");
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
        // add initial sensor for previous frame DAS
        startingGenome.AddNodeGene(new NodeGene(NodeGene.TYPE.SENSOR, 220));
        // add initial BIAS SENSOR - SHOULD ALWAYS BE SET TO 1
        startingGenome.AddNodeGene(new NodeGene(NodeGene.TYPE.SENSOR, 221));

        //add initial out nodes
        // 222 - null
        // 223 - left
        // 224 - down
        // 225 - right
        // 226 - a
        // 227 - b
        for (int i = 222; i < 228; i++) {
            startingGenome.AddNodeGene(new NodeGene(NodeGene.TYPE.OUTPUT, i));
        }

        for (int y = 0; y < 20; y++) {
            startingGenome.AddNodeGene(new NodeGene(NodeGene.TYPE.HIDDEN, 228 + y));
            startingGenome.AddConnectionGene(new ConnectionGene(228 + y, Random.Range(222, 228), Random.Range(-2f, 2f), true, History.Innovate()));
        }

        for (int y = 0; y < 20; y++) {
            for (int x = 0; x < 10; x++) {
                startingGenome.AddConnectionGene(new ConnectionGene(y * 10 + x + 1, 228 + y, Random.Range(0.5f, 2f), true, History.Innovate()));
            }
        }
    }

    public static void SaveGenome(Genome genome, string extPath) {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + extPath;

        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, genome);
        stream.Close();
    }

    public static Genome OpenGenome(string extPath) {
        string path = Application.persistentDataPath + "/" + extPath;

        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            Genome genome = formatter.Deserialize(stream) as Genome;
            stream.Close();
            return genome;
        } else {
            return null;
        }
    }
}
