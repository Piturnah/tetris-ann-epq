using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * This object reads keyboard input and produces visualisations for humans.
 */
[RequireComponent(typeof(TetrisEngine))]
public class HumanIO : MonoBehaviour
{
    TetrisEngine engine;

    private void Start()
    {
        // Obtain reference to the game engine
        engine = GetComponent<TetrisEngine>();
    }
    private void Update()
    {
        DetectRotationIO();
        DetectInput();
    }

    void DetectInput()
    {
        engine.buttonInfo.lButton = Input.GetKey(KeyCode.LeftArrow); // LR Input
        engine.buttonInfo.rbutton = Input.GetKey(KeyCode.RightArrow);

        engine.buttonInfo.dButton = Input.GetKey(KeyCode.DownArrow); // Soft-drop input
    }

    void DetectRotationIO()
    {
        engine.buttonInfo.aButton = engine.buttonInfo.bButton = false;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            engine.RotateTetromino(-1);
            engine.buttonInfo.aButton = true;
        } else if (Input.GetKeyDown(KeyCode.X))
        {
            engine.RotateTetromino(1);
            engine.buttonInfo.bButton = true;
        }
    }

    
}
