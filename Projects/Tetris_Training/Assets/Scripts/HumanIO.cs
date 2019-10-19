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
        engine = gameObject.GetComponent<TetrisEngine>();
    }
    private void Update()
    {
        // Update the visible tiles
        UpdateTiles();
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

    void UpdateTiles()
    {
        // Destroy any existing tiles
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Instantiate a new tile if it is present in the engine's field array
        for (int x = 0; x < engine.field.GetLength(0); x++)
        {
            for (int y = 0; y < engine.field.GetLength(1); y++)
            {
                if (engine.viewField[x, y] == 1)
                {
                    GameObject newTile = Instantiate(tileObj, new Vector2(x, y), Quaternion.identity);
                    newTile.transform.parent = transform;
                }
            }
        }
    }
}
