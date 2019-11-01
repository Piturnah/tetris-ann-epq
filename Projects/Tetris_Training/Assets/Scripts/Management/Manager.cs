using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    [HideInInspector] public int startLevel;
    public GameObject humanIO;

    public void HumanPlay(int startLevelVal)
    {
        startLevel = startLevelVal;
        SceneManager.LoadScene(1);
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnHumanSceneLoaded;
    }

    void OnHumanSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Main"));
        HumanIO newHuman = Instantiate(humanIO, Vector3.zero, Quaternion.identity).GetComponent<HumanIO>();
        newHuman.GetComponent<TetrisEngine>().startLevel = startLevel;
    }
}
