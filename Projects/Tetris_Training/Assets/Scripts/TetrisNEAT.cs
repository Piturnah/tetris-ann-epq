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

    int popSize = 1024;
    int batchSize = 200;
    int batched = 0;
    public static int generation = 0;

    int popIteration = 0;
    int highScore = 0;

    int noAgentsPlaying = 0;
    int noAgentsInGen = 0;

    bool displayAlive = false;

    bool agentsPlaying = false;
    bool oneByOne = false;
    GameObject runningAgent;
    float[] scores;
    void Start()
    {
        scores = new float[popSize];
        if (Manager.gameType == Manager.GameType.Train) {
            eval = new Evaluator(popSize, startingGenome);
            genText.text = "Generation: " + generation.ToString("0000");
            if (!oneByOne) {
                genomeText.text = "No. species: " + eval.GetSpeciesAmount().ToString("0000");
            }
        }
        if (Manager.gameType == Manager.GameType.Display) {
            GameObject newAgent = MakeNewAgent(Manager.displayGenome);
            newAgent.GetComponent<EngineUI>().enabled = true;
        }
    }

    private void Update() {
        if (Manager.gameType == Manager.GameType.Train) { // || Manager.gameType == Manager.GameType.Display) {
            if (oneByOne) {
                if (!agentsPlaying) {
                    agentsPlaying = true;
                    if (popIteration < popSize) { // still in same generation
                        noAgentsInGen++;

                        runningAgent = MakeNewAgent(eval.genomes[popIteration]);
                        runningAgent.GetComponent<EngineUI>().enabled = true;

                        genomeText.text = "Genome: " + popIteration.ToString("0000");
                    }
                }
            }
            else {
                if (noAgentsPlaying < batchSize) {
                    agentsPlaying = true;
                    for (int i = noAgentsPlaying; i < batchSize; i++) {
                        noAgentsInGen++;
                        if (noAgentsInGen > popSize) {
                            break;
                        }
                        if (Manager.gameType == Manager.GameType.Train) {
                            noAgentsPlaying++;

                            GameObject newAgent = MakeNewAgent(eval.genomes[noAgentsInGen-1]);
                            
                            if (!displayAlive) {
                                newAgent.GetComponent<EngineUI>().enabled = true;
                                displayAlive = true;
                            }
                        }
                    }
                    if (Manager.gameType == Manager.GameType.Display) {
                        GameObject newAgent = MakeNewAgent(Manager.displayGenome);
                        newAgent.GetComponent<EngineUI>().enabled = true;
                    }
                }
            }
        }
    }

    void TakeScore(int score, GameObject deadObj, int droppedTetros, Genome deadGenome) {
        if (deadObj.GetComponent<EngineUI>().enabled) {
            displayAlive = false;
        }

        noAgentsPlaying--;
        //Debug.Log(noAgentsPlaying);
        //scores[eval.genomes.IndexOf(deadGenome)] = (score + 30 * droppedTetros);
        scores[eval.genomes.IndexOf(deadGenome)] = (score);
        Destroy(deadObj);
        popIteration++;
        if (noAgentsPlaying == 0) {
            batched++;
            agentsPlaying = false;

            noAgentsInGen = 0;
            generation++;

            eval.Evaluate(scores.ToArray().Select(x => (float)x).ToArray()); // evaluate and breed current genomes

            if (noAgentsInGen == popSize) {
                batched = 0;
            }
            popIteration = 0;

            genomeText.text = "No. species: " + eval.GetSpeciesAmount().ToString("0000");

            genText.text = "Generation: " + generation.ToString("0000");
        }
        if (oneByOne) {
            agentsPlaying = false;
        }
        

        if (score > highScore) {
            highScore = score;
            UpdateHighScore();
            print("Highscore of " + highScore.ToString() + "on index " + eval.genomes.IndexOf(deadGenome).ToString());
        }
    }

    void UpdateHighScore() {
        highScoreText.text = "Highscore: " + highScore.ToString("0000000");
    }

    public GameObject MakeNewAgent(Genome genome) {
        GameObject newAgent = Instantiate(agent, Vector3.zero, Quaternion.identity);
        if (Manager.gameType == Manager.GameType.Train) {
            newAgent.GetComponent<TetrisAgent>().neuralNet = new NeuralNetwork(genome);
        }

        newAgent.GetComponent<TetrisAgent>().died += TakeScore;

        return newAgent;
    }

    private void Awake() {
        if (Manager.gameType == Manager.GameType.Train) {
            // add initial sensors
            for (int i = 1; i < 5; i++) {
                startingGenome.AddNodeGene(new NodeGene(NodeGene.TYPE.SENSOR, i));
            }
            // add out node
            startingGenome.AddNodeGene(new NodeGene(NodeGene.TYPE.OUTPUT, 5));

            //startingGenome.AddNodeGene(new NodeGene(NodeGene.TYPE.HIDDEN, 6));

            //startingGenome.AddConnectionMutation();

            /*// add initial sensors for board
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

            startingGenome.AddConnectionGene(new ConnectionGene(221, 223, 2f, true, History.Innovate()));

            startingGenome.AddConnectionMutation();
            startingGenome.AddNodeMutation();

            for (int y = 0; y < 20; y++) {
                startingGenome.AddNodeGene(new NodeGene(NodeGene.TYPE.HIDDEN, 228 + y));
                startingGenome.AddConnectionGene(new ConnectionGene(228 + y, Random.Range(222, 228), Random.Range(-2f, 2f), true, History.Innovate()));
            }*/

/*            for (int y = 0; y < 20; y++) {
                for (int x = 0; x < 10; x++) {
                    startingGenome.AddConnectionGene(new ConnectionGene(y * 10 + x + 1, 228 + y, Random.Range(0.5f, 2f), true, History.Innovate()));
                }
            }*/
        }
    }

    public static void SaveGenome(Genome genome, string extPath) {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/new/" + extPath;

        FileStream stream = new FileStream(path, FileMode.Create);

        //Create new serialisable GenomeData object from the Genome to save it
        formatter.Serialize(stream, new GenomeData(genome));
        stream.Close();
    }

    public static Genome OpenGenome(string extPath, bool usingExtPath = true) {
        string path = (usingExtPath) ? Application.persistentDataPath + "/" + extPath : extPath;

        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            //Deserialise as a GenomeData object
            GenomeData genome = formatter.Deserialize(stream) as GenomeData;

            //Read GenomeData information to create new Genes for the Genome
            Genome returnGenome = new Genome();
            for (int i = 0; i < genome.nodeGeneData.Count; i+=3) {
                NodeGene newNode = new NodeGene((genome.nodeGeneData[i + 1] == 0) ? NodeGene.TYPE.HIDDEN : ((genome.nodeGeneData[i + 1] == 1) ? NodeGene.TYPE.OUTPUT : NodeGene.TYPE.SENSOR), genome.nodeGeneData[i + 2]);
                newNode.activation = (genome.nodeGeneData[i] == 0) ? NodeGene.ACTIVATION.SINE : ((genome.nodeGeneData[i] == 1) ? NodeGene.ACTIVATION.COSINE : NodeGene.ACTIVATION.SIGMOID);
                returnGenome.AddNodeGene(newNode);
            }
            for (int i = 0; i < genome.connectionGeneData.Count; i += 5) {
                ConnectionGene newConnection = new ConnectionGene((int)genome.connectionGeneData[i], (int)genome.connectionGeneData[i + 1], genome.connectionGeneData[i + 2], genome.connectionGeneData[i + 3] == 1, (int)genome.connectionGeneData[i + 4]);
                returnGenome.AddConnectionGene(newConnection);
            }

            stream.Close();
            return returnGenome;
        } else {
            return null;
        }
    }
}
