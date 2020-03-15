using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtons : MonoBehaviour
{
    Manager manager;

    private void Start() {
        manager = FindObjectOfType<Manager>().GetComponent<Manager>();
    }
    public void MenuButton()
    {
        manager.GoToMenu();
    }
}
