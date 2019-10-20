using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Controlls everything that is displayed to the user
 */
[RequireComponent(typeof(TetrisEngine))]
public class EngineUI : MonoBehaviour
{
    TetrisEngine engine;
    public GameObject tileObj;

    private void Start()
    {
        // Obtain reference to the game engine
        engine = GetComponent<TetrisEngine>();

        GenerateTilemap();
    }
    private void Update()
    {
        //UpdateTiles();
    }

    void GenerateTilemap()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int y  = 0; y < 20; y++)
            {
                GameObject newTile = Instantiate(tileObj, new Vector2(x, y), Quaternion.identity);
                newTile.transform.parent = transform;
            }
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
        for (int x = 0; x < engine.field.Length; x++)
        {
            for (int y = 0; y < engine.field[0].Length; y++)
            {
                if (engine.viewField[x][y] == 1)
                {
                    GameObject newTile = Instantiate(tileObj, new Vector2(x, y), Quaternion.identity);
                    newTile.transform.parent = transform;
                }
            }
        }
    }
}
