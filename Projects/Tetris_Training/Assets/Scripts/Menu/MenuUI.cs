using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SFB;

public class MenuUI : MonoBehaviour
{
    [SerializeField] Text startLevel;
    int startLevelVal;
    Manager manager;

    private void Start() {
        manager = FindObjectOfType<Manager>().GetComponent<Manager>();
    }

    public void UpdateLevelText(Slider slider)
    {
        startLevel.text = "Level: " + slider.value.ToString("00");
        startLevelVal = Mathf.RoundToInt(slider.value);
    }

    public void HumanButtonClicked()
    {
        manager.HumanPlay(startLevelVal);
    }
    public void TrainButtonClicked() {
        manager.StartTraining(startLevelVal);
    }
    public void AIPlayClicked() {
        string pathToTetro = StandaloneFileBrowser.OpenFilePanel("Open File", Application.persistentDataPath, "tetro", false)[0];
        manager.AIPlay(startLevelVal, TetrisNEAT.OpenGenome(pathToTetro, false));
    }
}
