using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRenderer : MonoBehaviour
{
    MeshRenderer appearance;
    public TetrisEngine engine;
    ColourPalettes palettes;

    public bool isNext;

    private void Start()
    {
        palettes = FindObjectOfType<ColourPalettes>().GetComponent<ColourPalettes>();
        appearance = GetComponent<MeshRenderer>();
        engine = FindObjectOfType<TetrisEngine>().GetComponent<TetrisEngine>();
        //engine.newLevel += UpdateColour;
        if (!isNext)
        {
            engine = GetComponentInParent<TetrisEngine>();

            engine.updateField += UpdateAppearance;
        }
        else
        {
            switch (engine.nextTetrominoIndex)
            {
                case 1: case 2: case 6:
                    appearance.material.color = palettes.palette[0][engine.score.level % palettes.palette[0].Length];
                    break;
                case 3: case 5:
                    appearance.material.color = palettes.palette[1][engine.score.level % palettes.palette[0].Length];
                    break;
                default:
                    appearance.material.color = palettes.palette[2][engine.score.level % palettes.palette[0].Length];
                    break;
            }
        }
    }

    private void OnDestroy() {
        engine.updateField -= UpdateAppearance;
    }

    void UpdateAppearance()
    {
        appearance.enabled = (engine.viewField[Mathf.RoundToInt(transform.position.x)][Mathf.RoundToInt(transform.position.y)] != 0);
        UpdateColour();
    }

    void UpdateColour()
    {
        if (appearance.enabled)
        {
            appearance.material.color = palettes.palette[engine.viewField[Mathf.RoundToInt(transform.position.x)][Mathf.RoundToInt(transform.position.y)] - 1][engine.score.level % palettes.palette[0].Length];
        }
    }
}
