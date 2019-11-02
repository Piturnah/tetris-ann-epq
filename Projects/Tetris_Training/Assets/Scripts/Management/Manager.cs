using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Manager : MonoBehaviour
{
    [HideInInspector] public int startLevel;
    public GameObject humanIO;
    public static int gameMode;
    string savePath;
    public static int humanHiScoreVal;

    public void HumanPlay(int startLevelVal)
    {
        startLevel = startLevelVal;
        SceneManager.LoadScene(1);
    }

    private void Start()
    {
        savePath = Application.persistentDataPath;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnHumanSceneLoaded;
        TetrisEngine.death += OnDeath;
        humanHiScoreVal = GetHumanHiScore();
        print(humanHiScoreVal);

    }

    void OnHumanSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        gameMode = 0;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Main"));
        HumanIO newHuman = Instantiate(humanIO, Vector3.zero, Quaternion.identity).GetComponent<HumanIO>();
        newHuman.GetComponent<TetrisEngine>().startLevel = startLevel;
    }

    void OnDeath(int score) // called when any tetris engine dies
    {
        if (gameMode == 0) // human is playing
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
