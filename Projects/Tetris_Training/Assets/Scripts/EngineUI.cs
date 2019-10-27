using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
 * Controlls everything that is displayed to the user
 */
[RequireComponent(typeof(TetrisEngine))]
public class EngineUI : MonoBehaviour
{
    TetrisEngine engine;
    public GameObject tileObj;

    [SerializeField] Text dasText;

    private void Start()
    {
        // Obtain reference to the game engine
        engine = GetComponent<TetrisEngine>();

        GenerateTilemap();
    }
    private void Update()
    {
        dasText.text = "DAS: " + engine.buttonInfo.dasCounter.ToString("00");
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
}
