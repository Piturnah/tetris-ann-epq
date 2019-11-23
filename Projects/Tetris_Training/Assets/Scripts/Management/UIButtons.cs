using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtons : MonoBehaviour
{
    public void MenuButton()
    {
        FindObjectOfType<Manager>().GetComponent<Manager>().GoToMenu();
    }
}
