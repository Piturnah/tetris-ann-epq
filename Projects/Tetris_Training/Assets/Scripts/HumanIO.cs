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

    [SerializeField] GameObject tileObj;

    private void Start()
    {
        // Obtain reference to the game engine
        engine = GetComponent<TetrisEngine>();
    }
    private void Update()
    {
        DetectRotationIO();
        DetectHorizontalInput();
    }

    void DetectHorizontalInput()
    {
        engine.buttonInfo.lButton = Input.GetKey(KeyCode.LeftArrow);
        engine.buttonInfo.rbutton = Input.GetKey(KeyCode.RightArrow);
    }

    void DetectRotationIO()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            engine.RotateTetromino(-1);
        } else if (Input.GetKeyDown(KeyCode.X))
        {
            engine.RotateTetromino(1);
        }
    }

    
}
