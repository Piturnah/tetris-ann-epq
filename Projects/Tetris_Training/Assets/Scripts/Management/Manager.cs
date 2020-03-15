using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Manager : MonoBehaviour
{
    static GameObject manager = null;
    [HideInInspector] public int startLevel;
    public GameObject humanIO;
    //public static int gameMode;
    string savePath;
    public static int humanHiScoreVal;
    public enum GameType {Human, Train, Display};
    public static GameType gameType;
    public static Genome displayGenome;

    public void HumanPlay(int startLevelVal)
    {
        startLevel = startLevelVal;
        gameType = GameType.Human;
        SceneManager.LoadScene(1);
    }
    public void StartTraining(int startLevelVal) {
        startLevel = startLevelVal;
        gameType = GameType.Train;
        SceneManager.LoadScene(1);
    }
    public void AIPlay(int startLevelVal, Genome genome) {
        startLevel = startLevelVal;
        gameType = GameType.Display;
        displayGenome = genome;
        SceneManager.LoadScene(1);
    }

    private void Awake()
    {
        if (manager != null && manager != gameObject)
        {
            Destroy(gameObject);
        }
        else
        {
            manager = gameObject;
        }
    }

    private void Start()
    {
        savePath = Application.persistentDataPath;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        humanHiScoreVal = GetHumanHiScore();

    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(0);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main")
        {
            humanHiScoreVal = GetHumanHiScore();
            if (gameType == GameType.Human) {
                HumanIO newHuman = Instantiate(humanIO, Vector3.zero, Quaternion.identity).GetComponent<HumanIO>();
                newHuman.GetComponent<TetrisEngine>().startLevel = startLevel;
                newHuman.GetComponent<TetrisEngine>().death += OnDeath;
            }
        }
    }

    void OnDeath(int score) // called when any tetris engine dies
    {
        if (gameType == GameType.Human) // human is playing
        {
            if (humanHiScoreVal < score)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(savePath + "/humanHiScore.bin", FileMode.OpenOrCreate);
                formatter.Serialize(stream, score);
                stream.Close();
            }
            FindObjectOfType<HumanIO>().GetComponent<HumanIO>().enabled = false; // disable further player input
        }
    }

    int GetHumanHiScore()
    {
        string path = savePath + "/humanHiScore.bin";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            int newScore = (int)formatter.Deserialize(stream);
            stream.Close();
            return newScore;
        }
        else
        {
            Debug.LogWarning("No highscore data");
            return 0;
        }
    }
}
