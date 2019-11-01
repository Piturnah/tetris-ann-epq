using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] Text startLevel;
    int startLevelVal;

    public void UpdateLevelText(Slider slider)
    {
        startLevel.text = "Level: " + slider.value.ToString("00");
        startLevelVal = Mathf.RoundToInt(slider.value);
    }

    public void HumanButtonClicked()
    {
        FindObjectOfType<Manager>().GetComponent<Manager>().HumanPlay(startLevelVal);
    }
}
