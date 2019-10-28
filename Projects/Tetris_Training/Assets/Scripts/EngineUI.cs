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
    [SerializeField] Image dasImage;
    [SerializeField] Image nesController;
    [SerializeField] GameObject devTools;

    [SerializeField] public Transform nextTetrominoHolder;

    Color unselectedCol;

    private void Start()
    {
        // Obtain reference to the game engine
        engine = GetComponent<TetrisEngine>();

        GenerateTilemap();

        engine.tetrominoSpawned += UpdateTetrominoHolder;
        UpdateTetrominoHolder();

        unselectedCol = nesController.transform.Find("MID").GetComponent<Image>().color;
    }
    private void Update()
    {
        dasText.text = "DAS: " + engine.buttonInfo.dasCounter.ToString("00");
        dasImage.fillAmount = engine.buttonInfo.dasCounter / 16f;

        UpdateControllerDisplay();

        if (Input.GetKeyUp(KeyCode.D))
        {
            devTools.SetActive(!devTools.activeInHierarchy);
        }
    }

    void UpdateControllerDisplay()
    {
        foreach (Transform button in nesController.transform)
        {
            button.GetComponent<Image>().color = unselectedCol;
        }

        Color highlightedColour = new Color(227 / 255f, 186 / 255f, 50 / 255f);
        if (engine.buttonInfo.lButton)
        {
            nesController.transform.Find("LEFT").GetComponent<Image>().color = highlightedColour;
        }
        if (engine.buttonInfo.rbutton)
        {
            nesController.transform.Find("RIGHT").GetComponent<Image>().color = highlightedColour;
        }
        if (engine.buttonInfo.dButton)
        {
            nesController.transform.Find("DOWN").GetComponent<Image>().color = highlightedColour;
        }
        if (engine.buttonInfo.aButton)
        {
            nesController.transform.Find("A").GetComponent<Image>().color = highlightedColour;
        }
        if (engine.buttonInfo.bButton)
        {
            nesController.transform.Find("B").GetComponent<Image>().color = highlightedColour;
        }
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

    void UpdateTetrominoHolder()
    {
        foreach (Transform child in nextTetrominoHolder)
        {
            Destroy(child.gameObject);
        }

        int[,,] tetrominoPool = Tetrominoes.GetTetrominoFromIndex(engine.nextTetrominoIndex);

        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (engine.Slicer3D(tetrominoPool, 0)[x,y] != 0)
                {
                    Vector2 spawnPos = new Vector2(nextTetrominoHolder.position.x -2 + x, nextTetrominoHolder.position.y - 1 + y);
                    GameObject newTile = Instantiate(tileObj, spawnPos, Quaternion.identity);
                    newTile.GetComponent<TileRenderer>().isNext = true;
                    newTile.transform.parent = nextTetrominoHolder;
                    newTile.GetComponent<TileRenderer>().engine = engine;
                }
            }
        }
    }
}
