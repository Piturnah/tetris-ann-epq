using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.Mathematics;


/*
 * Controlls everything that is displayed to the user
 */
[RequireComponent(typeof(TetrisEngine))]
public class EngineUI : MonoBehaviour
{
    TetrisEngine engine;
    Objects obj;
    const float _FRAME_RATE = 60.098813897441f;

    [HideInInspector] public int yLock;

    int previousUpdateFrame;

    Color unselectedCol;
    Color bgMain = new Color(0.1176471f, 0.1254902f, 0.1333333f);
    Color highlightedColour = new Color(227 / 255f, 186 / 255f, 50 / 255f);
    Color aaaah = new Color(055f, 055f, 055f);

    private void Start()
    {
        // Obtain reference to the game engine
        engine = GetComponent<TetrisEngine>();
        obj = FindObjectOfType<Objects>().GetComponent<Objects>();

        obj.scoreText = GameObject.Find("Score").GetComponent<Text>();
        GenerateTilemap();

        if (Manager.gameType == Manager.GameType.Human || Manager.gameType == Manager.GameType.Display) {
            engine.death += OnDeath;
        }

        engine.nextAnimFrame += Flash;
        engine.tetrominoSpawned += UpdateTetrominoHolder;
        UpdateTetrominoHolder();

        unselectedCol = obj.nesController.transform.Find("MID").GetComponent<Image>().color;
    }
    private void Update()
    {
        if (obj.devTools.activeInHierarchy)
        {
            obj.dasText.text = "DAS: " + engine.buttonInfo.dasCounter.ToString("00");
            obj.dasImage.fillAmount = engine.buttonInfo.dasCounter / 16f;
            string hexString = engine.frameCounter.ToString("x8");
            obj.frameCounter.text = "FRAME: 0x " + string.Format("{0} {1}", hexString.Substring(0, 4), hexString.Substring(4, 4));
            obj.areEnabled.enabled = engine.are;
            obj.lockPosTxt.text = "LOCK: " + yLock.ToString("00");
            obj.sdText.text = "SOFT: " + engine.buttonInfo.softDropCounter.ToString("00");
            obj.sdImage.fillAmount = engine.buttonInfo.softDropCounter / 16f;
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            obj.devTools.SetActive(!obj.devTools.activeInHierarchy);
        }

        obj.scoreText.text = "Score: " + engine.score.score.ToString("000000");
        obj.levelText.text = "Level: " + engine.score.level.ToString("00");
        obj.linesText.text = "Lines: " + engine.score.lines.ToString("00");
    }

    void UpdateControllerDisplay()
    {
        foreach (Transform button in obj.nesController.transform)
        {
            button.GetComponent<Image>().color = unselectedCol;
        }

        
        if (engine.buttonInfo.lButton)
        {
            obj.nesController.transform.Find("LEFT").GetComponent<Image>().color = highlightedColour;
        }
        if (engine.buttonInfo.rbutton)
        {
            obj.nesController.transform.Find("RIGHT").GetComponent<Image>().color = highlightedColour;
        }
        if (engine.buttonInfo.dButton)
        {
            obj.nesController.transform.Find("DOWN").GetComponent<Image>().color = highlightedColour;
        }
        if (engine.buttonInfo.aButton)
        {
            obj.nesController.transform.Find("A").GetComponent<Image>().color = highlightedColour;
        }
        if (engine.buttonInfo.bButton)
        {
            obj.nesController.transform.Find("B").GetComponent<Image>().color = highlightedColour;
        }
    }

    public void UpdateControllerActivations(float[] activations) {
        float activationsSum = 0;
        foreach (float f in activations) {
            activationsSum += f;
        }
        for (int i = 0; i < activations.Length; i++) {
            activations[i] = activations[i] / activationsSum;
        }
        obj.nesController.transform.Find("LEFTa").GetComponent<Image>().color = Color.Lerp(unselectedCol, highlightedColour, activations[1]);
        obj.nesController.transform.Find("RIGHTa").GetComponent<Image>().color = Color.Lerp(unselectedCol, highlightedColour, activations[3]);
        obj.nesController.transform.Find("DOWNa").GetComponent<Image>().color = Color.Lerp(unselectedCol, highlightedColour, activations[2]);new Color(1, 1, 1, activations[2]);
        obj.nesController.transform.Find("Aa").GetComponent<Image>().color = Color.Lerp(unselectedCol, highlightedColour, activations[4]);new Color(1, 1, 1, activations[4]);
        obj.nesController.transform.Find("Ba").GetComponent<Image>().color = Color.Lerp(unselectedCol, highlightedColour, activations[5]);new Color(1, 1, 1, activations[5]);
    }

    void GenerateTilemap()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int y  = 0; y < 20; y++)
            {
                GameObject newTile = Instantiate(obj.tileObj, new Vector2(x, y), Quaternion.identity);
                newTile.transform.parent = transform;
            }
        }
    }

    void UpdateTetrominoHolder()
    {
        foreach (Transform child in obj.nextTetrominoHolder)
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
                    Vector2 spawnPos = new Vector2(obj.nextTetrominoHolder.position.x -2 + x, obj.nextTetrominoHolder.position.y - 1 + y);
                    GameObject newTile = Instantiate(obj.tileObj, spawnPos, Quaternion.identity);
                    newTile.GetComponent<TileRenderer>().isNext = true;
                    newTile.transform.parent = obj.nextTetrominoHolder;
                    newTile.GetComponent<TileRenderer>().engine = engine;
                }
            }
        }
    }

    void Flash()
    {
        StartCoroutine(SaviorOfTheUniverse());
    }

    IEnumerator SaviorOfTheUniverse()
    {
        Camera.main.backgroundColor = aaaah;
        yield return new WaitForSeconds(1 / _FRAME_RATE);
        Camera.main.backgroundColor = bgMain;
    }

    void OnDeath(int score)
    {
        obj = FindObjectOfType<Objects>().GetComponent<Objects>(); // don't know why I have to do this again.
        obj.deathMenu.transform.Find("Score").GetComponent<Text>().text = "Score: " + score.ToString("000000");
        obj.deathMenu.SetActive(true);
    }
}
